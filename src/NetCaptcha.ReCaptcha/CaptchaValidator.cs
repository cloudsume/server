namespace NetCaptcha.ReCaptcha;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

internal sealed class CaptchaValidator : CaptchaValidator<ReCaptchaValidation>
{
    private readonly ReCaptchaOptions options;
    private readonly HttpClient client;

    public CaptchaValidator(IOptions<ReCaptchaOptions> options, HttpClient client)
    {
        this.options = options.Value;
        this.client = client;
    }

    public override async ValueTask<bool> ExecuteAsync(ReCaptchaValidation @params, CancellationToken cancellationToken = default)
    {
        // Set up a request.
        var form = new List<KeyValuePair<string, string>>(3)
        {
            new("secret", this.options.SecretKey),
            new("response", @params.Token),
        };

        if (@params.ClientAddress is { } clientAddress)
        {
            form.Add(new("remoteip", clientAddress.ToString()));
        }

        // Send the request.
        using var request = new FormUrlEncodedContent(form);
        using var response = await this.client.PostAsync("https://www.google.com/recaptcha/api/siteverify", request, cancellationToken);
        var body = await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<VerifyResponse>(body);

        if (result == null)
        {
            throw new ExternalServiceException("Unexpected response from reCAPTCHA endpoint.");
        }

        // Verify response.
        if (!result.Success)
        {
            return false;
        }

        if (result.Score is { } score)
        {
            if (@params is not ReCaptchaValidationV3 v3)
            {
                throw new ArgumentException($"The value must be {typeof(ReCaptchaValidationV3)} if the configured key is V3.", nameof(@params));
            }
            else if (score < (v3.MinScore ?? this.options.MinScore) || result.Action != v3.Action)
            {
                return false;
            }
        }

        return true;
    }

    private sealed class VerifyResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("score")]
        public double? Score { get; set; }

        [JsonPropertyName("action")]
        public string? Action { get; set; }

        [JsonPropertyName("challenge_ts")]
        public DateTimeOffset ChallengeTimestamp { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }

        [JsonPropertyName("apk_package_name")]
        public string? AndroidPackageName { get; set; }

        [JsonPropertyName("error-codes")]
        public ICollection<string> ErrorCodes { get; set; } = new List<string>();
    }
}
