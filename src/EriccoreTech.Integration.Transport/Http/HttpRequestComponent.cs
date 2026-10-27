using System.Text;
using System.Text.Json;
using EriccoreTech.Integration.Contracts;
using EriccoreTech.Integration.Core;
using Microsoft.Extensions.Options;

namespace EriccoreTech.Integration.Transport {
    // Calls a web service. Step input: { "url": "...", "method": "GET", "headers": { }, "body": { } }.
    // Output: { "statusCode": 200, "body": ... }. Non-2xx responses fail the step so MaxAttempts retries them.
    public sealed class HttpRequestComponent : IIntegrationComponent {

        public const string ClientName = "integration-http";

        private static readonly HashSet<string> AllowedMethods = new(StringComparer.OrdinalIgnoreCase) {
            "GET", "POST", "PUT", "PATCH", "DELETE"
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpComponentOptions _options;

        public HttpRequestComponent(IHttpClientFactory httpClientFactory, IOptions<HttpComponentOptions> options) {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public string Type => "http";

        public async Task<StepResult> ExecuteAsync(StepContext context, CancellationToken cancellationToken) {
            var input = context.Input;
            if (input.ValueKind != JsonValueKind.Object || !input.TryGetProperty("url", out var urlElement)
                || urlElement.ValueKind != JsonValueKind.String) {
                return StepResult.Fail("http: input must be an object with a string 'url'.");
            }

            // Fail closed: https only, and only to hosts the operator listed. Redirects are off so they cannot bypass this.
            if (!Uri.TryCreate(urlElement.GetString(), UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps) {
                return StepResult.Fail("http: url must be an absolute https url.");
            }
            if (!_options.AllowedHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase)) {
                return StepResult.Fail($"http: host '{uri.Host}' is not in HttpComponent:AllowedHosts.");
            }

            var methodName = input.TryGetProperty("method", out var methodElement) && methodElement.ValueKind == JsonValueKind.String
                ? methodElement.GetString()!
                : "GET";
            if (!AllowedMethods.Contains(methodName)) {
                return StepResult.Fail($"http: method '{methodName}' is not supported.");
            }

            using var request = new HttpRequestMessage(new HttpMethod(methodName.ToUpperInvariant()), uri);
            if (input.TryGetProperty("body", out var body) && body.ValueKind != JsonValueKind.Null) {
                request.Content = new StringContent(body.GetRawText(), Encoding.UTF8, "application/json");
            }
            if (input.TryGetProperty("headers", out var headers) && headers.ValueKind == JsonValueKind.Object) {
                foreach (var header in headers.EnumerateObject()) {
                    if (header.Value.ValueKind != JsonValueKind.String) {
                        return StepResult.Fail($"http: header '{header.Name}' must be a string.");
                    }
                    if (!request.Headers.TryAddWithoutValidation(header.Name, header.Value.GetString())
                        && request.Content is not null) {
                        request.Content.Headers.TryAddWithoutValidation(header.Name, header.Value.GetString());
                    }
                }
            }

            var client = _httpClientFactory.CreateClient(ClientName);
            client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            try {
                using var response = await client.SendAsync(request, cancellationToken);
                var text = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode) {
                    return StepResult.Fail($"http: {(int)response.StatusCode} {response.ReasonPhrase} from {uri.Host}.");
                }
                return StepResult.Ok(JsonSerializer.SerializeToElement(new {
                    statusCode = (int)response.StatusCode,
                    body = ParseBody(text)
                }));
            } catch (HttpRequestException ex) {
                return StepResult.Fail($"http: {ex.Message}");
            } catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested) {
                return StepResult.Fail($"http: request to {uri.Host} timed out.");
            }
        }

        private static JsonElement ParseBody(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return JsonSerializer.SerializeToElement((string?)null);
            }
            try {
                using var document = JsonDocument.Parse(text);
                return document.RootElement.Clone();
            } catch (JsonException) {
                return JsonSerializer.SerializeToElement(text);
            }
        }

    }
}
