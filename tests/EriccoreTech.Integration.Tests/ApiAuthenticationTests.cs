using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EriccoreTech.Integration.Tests {
    public class ApiAuthenticationTests : IDisposable {

        public ApiAuthenticationTests() {
            // Program.cs reads configuration while building, so environment variables are the reliable way in.
            Environment.SetEnvironmentVariable("Authentication__Authority", "https://auth.test.invalid");
            Environment.SetEnvironmentVariable("Authentication__Audience", "integration-tests");
        }

        public void Dispose() {
            Environment.SetEnvironmentVariable("Authentication__Authority", null);
            Environment.SetEnvironmentVariable("Authentication__Audience", null);
        }

        [Fact]
        public async Task Health_IsAnonymous() {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            var response = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("/components")]
        [InlineData("/management/channels")]
        public async Task Endpoints_RequireAuthentication(string path) {
            using var factory = new WebApplicationFactory<Program>();
            using var client = factory.CreateClient();

            var response = await client.GetAsync(path);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void MissingAuthority_RefusesToStart() {
            Environment.SetEnvironmentVariable("Authentication__Authority", null);
            using var factory = new WebApplicationFactory<Program>();

            Assert.Throws<InvalidOperationException>(() => factory.CreateClient());
        }

    }
}
