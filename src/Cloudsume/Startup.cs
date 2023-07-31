namespace Cloudsume;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Amazon.SimpleSystemsManagement.Model;
using Candidate.Server;
using Cloudsume.Analytics;
using Cloudsume.DataManagers;
using Cloudsume.Stripe;
using global::Cassandra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthenticationSchemes = Cloudsume.Identity.AuthenticationSchemes;

internal sealed class Startup
{
    private readonly IConfiguration config;
    private readonly IWebHostEnvironment host;

    public Startup(IConfiguration config, IWebHostEnvironment host)
    {
        this.config = config;
        this.host = host;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Application foundation.
        services.AddIdentity();
        services.AddCloudsumeServices(this.config.GetSection("Services"));

        switch (this.config["Captcha:KeyProvider"])
        {
            case "Inline":
                services.AddCaptcha().AddReCaptcha(options =>
                {
                    var key = this.config["Captcha:KeyProviderOptions:Value"];

                    if (string.IsNullOrEmpty(key))
                    {
                        throw new Exception("No Captcha:KeyProviderOptions:Value has been set.");
                    }

                    options.SecretKey = key;
                    options.MinScore = this.config.GetValue<double>("Captcha:MinScore");
                });
                break;
            case "AWS":
                services.AddCaptcha().AddReCaptcha(options =>
                {
                    options.SecretKey = this.GetAwsSecret(this.config["Captcha:KeyProviderOptions:SecretId"]);
                    options.MinScore = this.config.GetValue<double>("Captcha:MinScore");
                });
                break;
            default:
                throw new Exception("Captcha:KeyProvider configuration has unknown value.");
        }

        switch (this.config["Stats:Repository"])
        {
            case "AWS":
                services.AddAwsStatsRepository(this.config.GetSection("Stats:RepositoryOptions"));
                break;
            case "":
            case null:
                services.AddNullStatsRepository();
                break;
            default:
                throw new Exception("Unknown stats repository.");
        }

        // Application options.
        services.AddOptions<ApplicationOptions>().Bind(this.config.GetSection("Application")).ValidateDataAnnotations();
        services.AddOptions<Options.PaymentReceivingOptions>().Bind(this.config.GetSection("Payment:Receiving")).ValidateDataAnnotations();
        services.AddOptions<Options.TemplatePriceConstraintOptions>().Bind(this.config.GetSection("Template:PriceConstraints")).ValidateDataAnnotations();

        // Cassandra services.
        services.AddOptions<CassandraOptions>().Bind(this.config.GetSection("Cassandra")).ValidateDataAnnotations();
        services.AddSingleton(this.CreateCassandraCluster);
        services.AddSingleton(this.CreateCassandraSession);
        services.AddSingleton<Cassandra.IReadConsistencyProvider, Cassandra.ReadConsistencyProvider>();
        services.AddCassandraRepositories();

        // Resume services.
        services.AddResume();

        switch (this.config["Template:WorkspaceRepository"])
        {
            case "FileSystem":
                services.AddFileSystemWorkspaceAssetRepository(this.config.GetSection("Template:WorkspaceRepositoryOptions"));
                break;
            case "AWS":
                services.AddAwsWorkspaceAssetRepository(this.config.GetSection("Template:WorkspaceRepositoryOptions"));
                break;
            default:
                throw new ConfigurationException("Unknow template workspace repository.");
        }

        switch (this.config["Template:WorkspacePreview"])
        {
            case "FileSystem":
                services.AddFileSystemWorkspacePreviewRepository(this.config.GetSection("Template:WorkspacePreviewOptions"));
                break;
            case "AWS":
                services.AddAwsWorkspacePreviewRepository(this.config.GetSection("Template:WorkspacePreviewOptions"));
                break;
            default:
                throw new ConfigurationException("Unknow template workspace preview repository.");
        }

        switch (this.config["SampleData:Photo:Repository"])
        {
            case "FileSystem":
                services.AddFileSystemSamplePhotoRepository(this.config.GetSection("SampleData:Photo:RepositoryOptions"));
                break;
            case "AWS":
                services.AddAwsSamplePhotoRepository(this.config.GetSection("SampleData:Photo:RepositoryOptions"));
                break;
            default:
                throw new Exception("Unknow sample photo repository.");
        }

        // Financial services.
        services.AddFinancial();
        services.AddStripe(this.ConfigureStripe);

        // Analytic services.
        services.AddSingleton<IUserActivitySerializer, UserActivitySerializer>();

        // Internal services.
        services.AddSingleton<IDataManager, AddressManager>();
        services.AddSingleton<IDataManager, CertificateManager>();
        services.AddSingleton<IDataManager, EducationManager>();
        services.AddSingleton<IDataManager, EmailAddressManager>();
        services.AddSingleton<IDataManager, ExperienceManager>();
        services.AddSingleton<IDataManager, GitHubManager>();
        services.AddSingleton<IDataManager, HeadlineManager>();
        services.AddSingleton<IDataManager, LanguageManager>();
        services.AddSingleton<IDataManager, LinkedInManager>();
        services.AddSingleton<IDataManager, MobileManager>();
        services.AddSingleton<IDataManager, NameManager>();
        services.AddSingleton<IDataManager, PhotoManager>();
        services.AddSingleton<IDataManager, SkillManager>();
        services.AddSingleton<IDataManager, SummarManager>();
        services.AddSingleton<IDataManager, WebsiteManager>();

        services.AddTransient<DataOperations.IDataOperationSerializer, DataOperations.DataOperationSerializer>();
        services.AddTransient<DataOperations.IGlobalOperationSerializer, DataOperations.GlobalOperationSerializer>();
        services.AddTransient<DataOperations.ISampleOperationSerializer, DataOperations.SampleOperationSerializer>();

        if (this.host.IsDevelopment())
        {
            services.AddHostedService<DevelopmentEnvironmentBootstrapper>();
        }

        services.AddSingleton<IResumeHelper, ResumeHelper>();

        this.ConfigureResumeTemplate(services);
        services.AddResumeBuilder();
        this.ConfigureResumeThumbnail(services);
        this.ConfigureResumePhoto(services);

        // ASP.NET Core services.
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supported = new[] { "en-US", "th-TH" };

            options.AddSupportedCultures(supported);
            options.AddSupportedUICultures(supported);
            options.SetDefaultCulture(supported[0]);
        });

        this.ConfigureDataProtection(services);

        services
            .AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new Binders.AssetNameBinderProvider());
                options.ModelBinderProviders.Insert(0, new Binders.LinkIdBinderProvider());

                // We want to treat plain string as a JSON string.
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
            })
            .AddJsonOptions(options => options.JsonSerializerOptions.AddSystemTypeConverters())
            .AddDataAnnotationsLocalization();

        services.AddCors(options =>
        {
            var config = this.config.GetSection("Cors").Get<CorsOptions>();

            options.AddDefaultPolicy(builder =>
            {
                if (config.Origins == null || !config.Origins.Any())
                {
                    throw new ConfigurationException("No CORS origins is specified.");
                }

                builder.WithOrigins(config.Origins.ToArray());
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.SetPreflightMaxAge(TimeSpan.FromSeconds(600)); // https://stackoverflow.com/a/40373949/1829232
            });
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Combined";
                options.DefaultChallengeScheme = AuthenticationSchemes.UltimaAccount;
                options.DefaultForbidScheme = AuthenticationSchemes.UltimaAccount;
            })
            .AddPolicyScheme("Combined", null, options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    // Get Bearer Token.
                    var authorization = context.Request.Headers.Authorization.FirstOrDefault();

                    if (string.IsNullOrEmpty(authorization))
                    {
                        return AuthenticationSchemes.UltimaAccount;
                    }

                    if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        return AuthenticationSchemes.UltimaAccount;
                    }

                    var token = authorization["Bearer ".Length..].Trim();

                    if (string.IsNullOrEmpty(token))
                    {
                        return AuthenticationSchemes.UltimaAccount;
                    }

                    // Check token issuer.
                    var handler = new JwtSecurityTokenHandler();
                    JwtSecurityToken jwt;

                    try
                    {
                        jwt = handler.ReadJwtToken(token);
                    }
                    catch (ArgumentException)
                    {
                        return AuthenticationSchemes.UltimaAccount;
                    }

                    return jwt.Issuer == "cloudsume" ? AuthenticationSchemes.Guest : AuthenticationSchemes.UltimaAccount;
                };
            })
            .AddJwtBearer(AuthenticationSchemes.Guest, options =>
            {
                // The configurations here will effect on token generation too. See GuestSessionsController for more details.
                options.TokenValidationParameters.ValidIssuers = new[] { "cloudsume" };
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.RequireExpirationTime = false;

                // All key must be exactly 64 bytes. It is a key for HMAC SHA-256.
                // The newest key must be on the top.
                var keys = new List<SymmetricSecurityKey>();

                switch (this.config["Guest:TokenKey:Provider"])
                {
                    case "Inline":
                        foreach (var key in this.config.GetSection("Guest:TokenKey:ProviderOptions:Keys").Get<IEnumerable<InlineGuestTokenKey>>())
                        {
                            var id = key.Id;
                            var secret = key.Secret;

                            if (id == Guid.Empty)
                            {
                                throw new ConfigurationException("No identifier is specified for guest token key.");
                            }

                            if (string.IsNullOrEmpty(secret))
                            {
                                throw new ConfigurationException($"No secret is specified for guest token key {id}.");
                            }

                            keys.Add(new(Convert.FromBase64String(secret))
                            {
                                KeyId = id.ToString(),
                            });
                        }

                        break;
                    case "AWS":
                        foreach (var key in this.config.GetSection("Guest:TokenKey:ProviderOptions:Keys").Get<IEnumerable<AwsGuestTokenKey>>())
                        {
                            var id = key.Id;
                            var secretId = key.SecretId;

                            if (id == Guid.Empty)
                            {
                                throw new ConfigurationException("No identifier is specified for guest token key.");
                            }

                            if (string.IsNullOrEmpty(secretId))
                            {
                                throw new ConfigurationException($"No secret identifier is specified for guest token key {id}.");
                            }

                            keys.Add(new(Convert.FromBase64String(this.GetAwsSecret(secretId)))
                            {
                                KeyId = id.ToString(),
                            });
                        }

                        break;
                    default:
                        throw new ConfigurationException("Unknown token key provider for guest session.");
                }

                options.TokenValidationParameters.IssuerSigningKeys = keys;
            })
            .AddJwtBearer(AuthenticationSchemes.UltimaAccount, options =>
            {
                var config = this.config.GetSection("Identity").Get<IdentityOptions>();

                if (string.IsNullOrWhiteSpace(config.Provider))
                {
                    throw new ConfigurationException("No identity provider has been configured.");
                }

                options.Authority = config.Provider;
                options.Audience = "cloudsume";
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters.SaveSigninToken = true;

                if (!this.host.IsProduction())
                {
                    options.BackchannelHttpHandler = CreateInsecureHttpHandler();
                }

                // https://github.com/dotnet/aspnetcore/blob/main/src/Security/Authentication/JwtBearer/src/JwtBearerPostConfigureOptions.cs
                options.Backchannel = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler())
                {
                    Timeout = options.BackchannelTimeout,
                    MaxResponseContentBufferSize = 1024 * 1024 * 10, // 10 MB
                };

                if (this.host.IsProduction())
                {
                    options.Backchannel.DefaultRequestVersion = new Version(2, 0);
                }
            });

        services.AddAuthorization(this.ConfigureAuthorization);
        services.AddLocalization(this.ConfigureLocalization);
        services.AddHealthChecks();

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // System services.
        this.ConfigureHttpClient(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
        app.UseRequestLocalization();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health");
        });
    }

    private static HttpMessageHandler CreateInsecureHttpHandler() => new SocketsHttpHandler()
    {
        SslOptions = new()
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
        },
    };

    private Cluster CreateCassandraCluster(IServiceProvider services)
    {
        var options = services.GetRequiredService<IOptions<CassandraOptions>>().Value;
        var username = options.Username;
        var password = options.Password;
        var builder = Cluster.Builder().AddContactPoints(options.ContactPoints).WithDefaultKeyspace(options.Keyspace);

        if (username.Length > 0 && password != null)
        {
            var secret = password.Provider switch
            {
                CassandraOptions.PasswordProvider.Inline => GetInlinePassword(),
                CassandraOptions.PasswordProvider.AWS => GetAwsPassword(),
                _ => throw new InvalidOperationException($"Unknow Cassandra password provider '{password.Provider}'."),
            };

            builder.WithCredentials(username, secret);

            string GetInlinePassword()
            {
                var options = GetOptions<CassandraOptions.InlinePassword>();
                var password = options.Password;

                return password ?? throw new InvalidOperationException($"No '{nameof(options.Password)}' is specified for Cassandra password.");
            }

            string GetAwsPassword()
            {
                var options = GetOptions<CassandraOptions.AwsPassword>();
                var secretId = options.SecretId;

                if (secretId == null)
                {
                    throw new InvalidOptionException($"No '{nameof(options.SecretId)}' is specified for Cassandra password.");
                }

                return this.GetAwsSecret(secretId);
            }

            T GetOptions<T>()
            {
                var options = password.ProviderOptions;

                if (options == null)
                {
                    throw new InvalidOptionException($"No '{nameof(password.ProviderOptions)}' is specified for Cassandra password.");
                }

                return options.Get<T>();
            }
        }

        return builder.Build();
    }

    private ISession CreateCassandraSession(IServiceProvider services)
    {
        var cluster = services.GetRequiredService<Cluster>();

        if (this.host.IsDevelopment())
        {
            for (var i = 0; ; i++)
            {
                try
                {
                    return cluster.ConnectAndCreateDefaultKeyspaceIfNotExists(ReplicationStrategies.CreateSimpleStrategyReplicationProperty(1));
                }
                catch (NoHostAvailableException)
                {
                    if (i < 3)
                    {
                        Thread.Sleep(1000 * 30 * (i + 1));
                        continue;
                    }

                    throw;
                }
            }
        }
        else
        {
            return cluster.Connect();
        }
    }

    private void ConfigureStripe(StripeOptions options)
    {
        options.Key = this.config["Stripe:Key:Provider"] switch
        {
            "Inline" => GetInlineKey(),
            "AWS" => GetAwsKey(),
            _ => throw new ConfigurationException("Unknown Stripe key provider."),
        };

        string GetInlineKey()
        {
            var key = this.config["Stripe:Key:ProviderOptions:Value"];

            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("No Stripe:Key:ProviderOptions:Value configuration has been set.");
            }

            return key;
        }

        string GetAwsKey() => this.GetAwsSecret(this.config["Stripe:Key:ProviderOptions:SecretId"]);
    }

    private string GetAwsSecret(string id)
    {
        using var client = new AmazonSecretsManagerClient();
        var request = new GetSecretValueRequest() { SecretId = id };
        var response = client.GetSecretValueAsync(request).Result;

        return response.SecretString;
    }

    private void ConfigureResumeTemplate(IServiceCollection services)
    {
        var options = this.config.GetSection("Template").Get<ResumeTemplateOptions>();

        // Asset.
        switch (options.AssetRepository)
        {
            case ResumeTemplateOptions.AssetRepositoryType.FileSystem:
                services.AddFileSystemResumeTemplateAssetRepository(RequireAssetRepositoryOptions());
                break;
            case ResumeTemplateOptions.AssetRepositoryType.AWS:
                services.AddAwsTemplateAssetRepository(RequireAssetRepositoryOptions());
                break;
            default:
                throw new InvalidOperationException("Unknow template asset repository.");
        }

        // Preview.
        switch (options.PreviewRepository)
        {
            case ResumeTemplateOptions.PreviewRepositoryType.FileSystem:
                services.AddFileSystemTemplatePreviewRepository(RequirePreviewRepositoryOptions());
                break;
            case ResumeTemplateOptions.PreviewRepositoryType.AWS:
                services.AddAwsTemplatePreviewRepository(RequirePreviewRepositoryOptions());
                break;
            default:
                throw new InvalidOptionException("Unknow template preview repository.");
        }

        IConfigurationSection RequireAssetRepositoryOptions()
        {
            if (options.AssetRepositoryOptions == null)
            {
                throw new InvalidOperationException("No options for resume template asset repository has been set.");
            }

            return options.AssetRepositoryOptions;
        }

        IConfigurationSection RequirePreviewRepositoryOptions()
        {
            if (options.PreviewRepositoryOptions == null)
            {
                throw new InvalidOperationException("No options for template preview repository has been set.");
            }

            return options.PreviewRepositoryOptions;
        }
    }

    private void ConfigureResumeThumbnail(IServiceCollection services)
    {
        var prefix = "Resume:Thumbnail";
        var options = this.config.GetSection(prefix).Get<ResumeThumbnailOptions>();

        // Repository.
        switch (options.Repository)
        {
            case ResumeThumbnailOptions.RepositoryType.FileSystem:
                services.AddFileSystemResumeThumbnailRepository(RequireRepositoryOptions());
                break;
            case ResumeThumbnailOptions.RepositoryType.AWS:
                services.AddAwsResumeThumbnailRepository(RequireRepositoryOptions());
                break;
            default:
                throw new InvalidOperationException($"Unsupported {prefix}:{nameof(options.Repository)}.");
        }

        IConfigurationSection RequireRepositoryOptions()
        {
            if (options.RepositoryOptions == null)
            {
                throw new InvalidOperationException("No options for resume repository has been set.");
            }

            return options.RepositoryOptions;
        }
    }

    private void ConfigureResumePhoto(IServiceCollection services)
    {
        var options = this.config.GetSection("Resume:Photo").Get<ResumePhotoOptions>();

        switch (options.Repository)
        {
            case ResumePhotoOptions.RepositoryType.FileSystem:
                services.AddFileSystemResumePhotoRepository(RequireRepositoryOptions());
                break;
            case ResumePhotoOptions.RepositoryType.AWS:
                services.AddAwsResumePhotoRepository(RequireRepositoryOptions());
                break;
            default:
                throw new InvalidOperationException("Unknow repository for resume photo.");
        }

        IConfigurationSection RequireRepositoryOptions()
        {
            if (options.RepositoryOptions == null)
            {
                throw new InvalidOperationException("No options for resume photo repository has been set.");
            }

            return options.RepositoryOptions;
        }
    }

    private void ConfigureDataProtection(IServiceCollection services)
    {
        var options = this.config.GetSection("DataProtection").Get<Candidate.Server.DataProtectionOptions>();
        var builder = services.AddDataProtection();

        switch (options.Provider)
        {
            case Candidate.Server.DataProtectionOptions.ProviderType.Default:
                break;
            case Candidate.Server.DataProtectionOptions.ProviderType.FileSystem:
                ConfigureFileSystem();
                break;
            case Candidate.Server.DataProtectionOptions.ProviderType.AWS:
                ConfigureAws();
                break;
            default:
                throw new InvalidOperationException($"Unknow data protection provider {options.Provider}.");
        }

        void ConfigureFileSystem()
        {
            var options = GetProviderOptions<Candidate.Server.DataProtectionOptions.FileSystemOptions>();

            if (options.Path == null)
            {
                throw new InvalidOperationException("No path to store data protection keys is configured.");
            }

            builder.PersistKeysToFileSystem(new DirectoryInfo(options.Path));
        }

        void ConfigureAws()
        {
            var options = GetProviderOptions<Candidate.Server.DataProtectionOptions.AwsOptions>();

            if (options.Path == null)
            {
                throw new InvalidOperationException("No path to store data protection keys is configured.");
            }

            builder.PersistKeysToAWSSystemsManager(options.Path);
        }

        T GetProviderOptions<T>()
        {
            if (options.ProviderOptions == null)
            {
                throw new InvalidOperationException("No data protection is configured.");
            }

            return options.ProviderOptions.Get<T>();
        }
    }

    private void ConfigureAuthorization(AuthorizationOptions options)
    {
        foreach (var field in typeof(AuthorizationPolicies).GetFields())
        {
            var attribute = field.GetCustomAttribute<AuthorizationPolicyAttribute>();

            if (attribute == null)
            {
                throw new Exception($"{typeof(AuthorizationPolicies)}.{field} does not have {typeof(AuthorizationPolicyAttribute)} applied.");
            }

            options.AddPolicy((string)field.GetRawConstantValue()!, b =>
            {
                b.RequireAuthenticatedUser();
                b.AddRequirements(new AuthorizationRequirement(attribute));
            });
        }
    }

    private void ConfigureLocalization(LocalizationOptions options)
    {
        options.ResourcesPath = "Resources";
    }

    private void ConfigureHttpClient(IServiceCollection services)
    {
        services.AddTransient<HttpClient>(p =>
        {
            var host = p.GetRequiredService<IHostEnvironment>();

            if (host.IsProduction())
            {
                return new HttpClient();
            }
            else
            {
                var handler = CreateInsecureHttpHandler();

                try
                {
                    return new HttpClient(handler, true);
                }
                catch
                {
                    handler.Dispose();
                    throw;
                }
            }
        });
    }

    private sealed class InlineGuestTokenKey
    {
        public Guid Id { get; set; }

        public string? Secret { get; set; }
    }

    private sealed class AwsGuestTokenKey
    {
        public Guid Id { get; set; }

        public string? SecretId { get; set; }
    }
}
