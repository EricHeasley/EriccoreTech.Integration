using System.Text.Json;
using EriccoreTech.IntegrationFlow.Model;
using EriccoreTech.IntegrationFlow.Engine;
using EriccoreTech.IntegrationFlow.Patterns;
using Xunit;

namespace EriccoreTech.IntegrationFlow.Tests {
    public sealed class WorkflowTests {

        [Fact]
        public void CompilerRejectsCycles() {
            var workflow = new WorkflowDefinition("cycle", [
                new("a", "echo", ["b"]), new("b", "echo", ["a"])]);
            Assert.Throws<ArgumentException>(() => WorkflowCompiler.Compile(workflow));
        }

        [Fact]
        public void CompilerRejectsMissingDependencies() {
            var workflow = new WorkflowDefinition("bad", [new("a", "echo", ["missing"])]);
            Assert.Throws<ArgumentException>(() => WorkflowCompiler.Compile(workflow));
        }

        [Fact]
        public async Task RunsDependentSteps() {
            var engine = new WorkflowEngine([new EchoComponent(), new UppercaseComponent()]);
            var workflow = new WorkflowDefinition("demo", [
                new("a", "echo"), new("b", "uppercase", ["a"])]);
            var result = await engine.ExecuteAsync(workflow,
                JsonSerializer.SerializeToElement(new { text = "hello" }));
            Assert.True(result.Steps["b"].Success);
            Assert.Equal("HELLO", result.Steps["b"].Output!.Value.GetProperty("text").GetString());
        }

        [Fact]
        public async Task RoutesByContent() {
            var component = new ContentBasedRouterComponent();
            var result = await component.ExecuteAsync(
                new StepContext(Guid.NewGuid(), JsonSerializer.SerializeToElement(new { priority = "high" }),
                    new Dictionary<string, StepResult>()), CancellationToken.None);
            Assert.Equal("priority", result.Output!.Value.GetProperty("route").GetString());
        }

    }
}
