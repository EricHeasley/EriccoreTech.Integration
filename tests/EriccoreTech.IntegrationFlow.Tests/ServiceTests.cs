using System.Text.Json;
using EriccoreTech.IntegrationFlow.Orchestration;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Core;
using EriccoreTech.IntegrationFlow.Transformation;
using EriccoreTech.IntegrationFlow.Transport;
using EriccoreTech.IntegrationFlow.Service;
using Xunit;

namespace EriccoreTech.IntegrationFlow.Tests {
    public sealed class ServiceTests {

        [Fact]
        public async Task WorkflowServiceExecutesDefinition() {
            var service = new WorkflowService(new ExecutionScheduler([new EchoComponent(), new UppercaseComponent()]));
            var definition = new WorkflowDefinition("demo", [new("a", "echo"), new("b", "uppercase", ["a"])]);

            var response = await service.ExecuteWorkflowAsync(new ExecuteWorkflowRequest(
                definition, JsonSerializer.SerializeToElement(new { text = "hi" })));

            Assert.True(response.Result.Steps["b"].Success);
        }

        [Fact]
        public async Task WorkflowServiceRejectsNullRequest() {
            var service = new WorkflowService(new ExecutionScheduler([]));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.ExecuteWorkflowAsync(null!));
        }

        [Fact]
        public void ComponentCatalogListsRegisteredTypes() {
            var service = new ComponentCatalogService([new EchoComponent(), new UppercaseComponent()]);
            var response = service.ListComponents(new ListComponentsRequest());
            Assert.Equal(new[] { "echo", "uppercase" }, response.ComponentTypes);
        }

        [Fact]
        public void ManagementServicePausesAndResumesChannel() {
            var service = new ManagementService(new ControlBus());

            Assert.True(service.PauseChannel(new PauseChannelRequest("jobs")).Paused);
            Assert.True(service.ListChannels(new ListChannelsRequest()).Channels["jobs"]);
            Assert.False(service.ResumeChannel(new ResumeChannelRequest("jobs")).Paused);
            Assert.False(service.ListChannels(new ListChannelsRequest()).Channels["jobs"]);
        }

        [Fact]
        public async Task MessagingServiceReturnsEnvelopeIdentifiers() {
            var service = new MessagingService(new InMemoryTransport());
            var payload = JsonSerializer.SerializeToElement(new { text = "hi" });

            var response = await service.PublishMessageAsync(new PublishMessageRequest("jobs", payload));

            Assert.NotEqual(Guid.Empty, response.MessageId);
            Assert.NotEqual(Guid.Empty, response.CorrelationId);
        }

    }
}
