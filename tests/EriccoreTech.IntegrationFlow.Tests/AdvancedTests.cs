using System.Text.Json;
using EriccoreTech.IntegrationFlow.Orchestration;
using EriccoreTech.IntegrationFlow.Management;
using EriccoreTech.IntegrationFlow.Processing;
using Xunit;

namespace EriccoreTech.IntegrationFlow.Tests {
    public class AdvancedTests {

        [Fact]
        public void SplitterAndAggregator() {
            var input = JsonSerializer.SerializeToElement(new[] { 1, 2, 3 });
            var parts = new Splitter().Split(input);
            Assert.Equal(3, parts.Count);
            Assert.Equal(3, new Aggregator().Aggregate(parts).GetArrayLength());
        }

        [Fact]
        public void Conditions() {
            var input = JsonSerializer.SerializeToElement(new { status = "ready" });
            Assert.True(ConditionEvaluator.Evaluate(input, new Condition("status", "eq", "ready")));
        }

        [Fact]
        public void ControlBus() {
            var bus = new Management.ControlBus();
            bus.Pause("jobs");
            Assert.True(bus.IsPaused("jobs"));
            bus.Resume("jobs");
            Assert.False(bus.IsPaused("jobs"));
        }

        [Fact]
        public void StateMachineRejectsInvalidTransition() {
            Assert.Throws<InvalidOperationException>(() =>
                ProcessStateMachine.Transition(ProcessStatus.Completed, ProcessStatus.Running));
        }

    }
}
