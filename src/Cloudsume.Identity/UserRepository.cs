namespace Cloudsume.Identity
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    internal sealed class UserRepository : IDisposable, IUserRepository
    {
        private readonly IOptionsMonitor<JwtBearerOptions> jwt;
        private readonly HttpClient http;
        private readonly DiscoveryCache discovery;

        public UserRepository(IOptionsMonitor<JwtBearerOptions> jwt, IWebHostEnvironment host)
        {
            this.jwt = jwt;

            // Setup client.
            if (host.IsProduction())
            {
                this.http = new ProductionClient();
            }
            else
            {
                var handler = CreateInsecureHttpHandler();

                try
                {
                    this.http = new HttpClient(handler, true);
                }
                catch
                {
                    handler.Dispose();
                    throw;
                }
            }

            try
            {
                this.discovery = new DiscoveryCache(this.Jwt.Authority, () => this.http);
            }
            catch
            {
                this.http.Dispose();
                throw;
            }
        }

        private JwtBearerOptions Jwt => this.jwt.Get(AuthenticationSchemes.UltimaAccount);

        public void Dispose()
        {
            this.http.Dispose();
        }

        public async Task<User?> GetAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
        {
            // Check if user is guest.
            var issuer = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Iss);

            if (issuer.Value == "cloudsume")
            {
                return null;
            }

            // Get access token to invoke OP.
            var token = (string?)principal.Identities.First().BootstrapContext;

            if (token == null)
            {
                throw new ArgumentException("No access token has been associated with the value.", nameof(principal));
            }

            // Get user info from OP.
            var oidc = await this.GetOidcAsync();
            var info = await this.http.GetUserInfoAsync(new() { Address = oidc.UserInfoEndpoint, Token = token }, cancellationToken);

            if (info.IsError)
            {
                throw new OidcProviderException(info.Error);
            }

            return new User(info.Claims);
        }

        public Guid GetId(ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                throw new ArgumentException($"The value does not contains {ClaimTypes.NameIdentifier}.", nameof(principal));
            }

            return Guid.Parse(claim.Value);
        }

        private static HttpMessageHandler CreateInsecureHttpHandler() => new SocketsHttpHandler()
        {
            SslOptions = new()
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true,
            },
        };

        private async Task<DiscoveryDocumentResponse> GetOidcAsync()
        {
            var response = await this.discovery.GetAsync();

            if (response.IsError)
            {
                throw new OidcProviderException(response.Error);
            }

            return response;
        }
    }
}
