using System.Net;
using System.Text.Json;
using EriccoreTech.Integration.Core;
using EriccoreTech.Integration.Transport;
using Microsoft.Extensions.Options;
using Xunit;

namespace EriccoreTech.Integration.Tests {
    public class HttpRequestComponentTests {

        private static HttpRequestComponent Create(HttpStatusCode status, string body, out StubHandler handler, params string[] allowedHosts) {
            handler = new StubHandler(status, body);
            var options = Options.Create(new HttpComponentOptions { AllowedHosts = allowedHosts });
            return new HttpRequestComponent(new StubFactory(handler), options);
        }

        private static StepContext Input(string json) {
            return new StepContext(Guid.NewGuid(), JsonDocument.Parse(json).RootElement.Clone(),
                new Dictionary<string, StepResult>());
        }

        [Fact]
        public async Task Get_ReturnsStatusAndParsedBody() {
            var component = Create(HttpStatusCode.OK, "{\"id\":7}", out var handler, "api.example.com");

            var result = await component.ExecuteAsync(Input("{\"url\":\"https://api.example.com/orders/7\"}"), CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal(200, result.Output!.Value.GetProperty("statusCode").GetInt32());
            Assert.Equal(7, result.Output.Value.GetProperty("body").GetProperty("id").GetInt32());
            Assert.Equal(HttpMethod.Get, handler.Request!.Method);
        }

        [Fact]
        public async Task Post_SendsJsonBodyAndHeaders() {
            var component = Create(HttpStatusCode.Created, "", out var handler, "api.example.com");

            var result = await component.ExecuteAsync(Input(
                "{\"url\":\"https://api.example.com/orders\",\"method\":\"post\",\"headers\":{\"X-Key\":\"abc\"},\"body\":{\"qty\":2}}"),
                CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal(HttpMethod.Post, handler.Request!.Method);
            Assert.Equal("abc", handler.Request.Headers.GetValues("X-Key").Single());
            Assert.Equal("{\"qty\":2}", handler.Body);
        }

        [Fact]
        public async Task HostNotAllowed_Fails() {
            var component = Create(HttpStatusCode.OK, "{}", out var handler, "api.example.com");

            var result = await component.ExecuteAsync(Input("{\"url\":\"https://evil.example.net/\"}"), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Null(handler.Request);
        }

        [Fact]
        public async Task NoAllowedHosts_RefusesEverything() {
            var component = Create(HttpStatusCode.OK, "{}", out var handler);

            var result = await component.ExecuteAsync(Input("{\"url\":\"https://api.example.com/\"}"), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Null(handler.Request);
        }

        [Fact]
        public async Task PlainHttp_Fails() {
            var component = Create(HttpStatusCode.OK, "{}", out var handler, "api.example.com");

            var result = await component.ExecuteAsync(Input("{\"url\":\"http://api.example.com/\"}"), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Null(handler.Request);
        }

        [Fact]
        public async Task NonSuccessStatus_Fails() {
            var component = Create(HttpStatusCode.ServiceUnavailable, "down", out _, "api.example.com");

            var result = await component.ExecuteAsync(Input("{\"url\":\"https://api.example.com/\"}"), CancellationToken.None);

            Assert.False(result.Success);
            Assert.Contains("503", result.Error);
        }

        [Fact]
        public async Task MissingUrl_Fails() {
            var component = Create(HttpStatusCode.OK, "{}", out _, "api.example.com");

            var result = await component.ExecuteAsync(Input("{}"), CancellationToken.None);

            Assert.False(result.Success);
        }

        private sealed class StubHandler : HttpMessageHandler {

            private readonly HttpStatusCode _status;
            private readonly string _responseBody;

            public StubHandler(HttpStatusCode status, string responseBody) {
                _status = status;
                _responseBody = responseBody;
            }

            public HttpRequestMessage? Request { get; private set; }

            public string? Body { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
                Request = request;
                Body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
                return new HttpResponseMessage(_status) { Content = new StringContent(_responseBody) };
            }

        }

        private sealed class StubFactory : IHttpClientFactory {

            private readonly HttpMessageHandler _handler;

            public StubFactory(HttpMessageHandler handler) {
                _handler = handler;
            }

            public HttpClient CreateClient(string name) {
                return new HttpClient(_handler, disposeHandler: false);
            }

        }

    }
}
