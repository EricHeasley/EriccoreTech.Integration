using System.Text.Json;
using EriccoreTech.IntegrationFlow.Core;

namespace EriccoreTech.IntegrationFlow.Engine {
    public sealed class WorkflowEngine {

        private readonly Dictionary<string, IIntegrationComponent> _components;

        public WorkflowEngine(IEnumerable<IIntegrationComponent> components) {
            _components = components.ToDictionary(c => c.Type, StringComparer.OrdinalIgnoreCase);
        }

        public async Task<WorkflowResult> ExecuteAsync(WorkflowDefinition definition,
            JsonElement input, CancellationToken cancellationToken = default, int maxConcurrency = 8) {
            if (maxConcurrency < 1) {
                throw new ArgumentOutOfRangeException(nameof(maxConcurrency));
            }

            var plan = WorkflowCompiler.Compile(definition);
            var executionId = Guid.NewGuid();
            var tasks = new Dictionary<string, Task<StepResult>>(StringComparer.OrdinalIgnoreCase);
            using var gate = new SemaphoreSlim(maxConcurrency);

            // Tasks are created in topological order, so dependency tasks always exist.
            foreach (var step in plan) {
                tasks[step.Id] = RunAsync(step);
            }

            await Task.WhenAll(tasks.Values);
            return new WorkflowResult(executionId, tasks.ToDictionary(k => k.Key, v => v.Value.Result));

            async Task<StepResult> RunAsync(StepDefinition step) {
                var dependencies = new Dictionary<string, StepResult>(StringComparer.OrdinalIgnoreCase);
                foreach (var dependency in step.DependsOn ?? []) {
                    dependencies[dependency] = await tasks[dependency];
                }

                if (dependencies.Values.Any(r => !r.Success)) {
                    return StepResult.Fail("A dependency failed.");
                }

                if (!_components.TryGetValue(step.Component, out var component)) {
                    return StepResult.Fail($"Component not registered: {step.Component}");
                }

                await gate.WaitAsync(cancellationToken);
                try {
                    for (int attempt = 1; attempt <= step.MaxAttempts; attempt++) {
                        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                        timeout.CancelAfter(TimeSpan.FromSeconds(step.TimeoutSeconds));
                        try {
                            var result = await component.ExecuteAsync(
                                new StepContext(executionId, input, dependencies), timeout.Token);
                            if (result.Success || attempt == step.MaxAttempts) {
                                return result;
                            }
                        } catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) {
                            if (attempt == step.MaxAttempts) {
                                return StepResult.Fail("Step timed out.");
                            }
                        } catch (Exception ex) when (ex is not OperationCanceledException) {
                            if (attempt == step.MaxAttempts) {
                                return StepResult.Fail(ex.Message);
                            }
                        }
                    }

                    return StepResult.Fail("Attempts exhausted.");
                } finally {
                    gate.Release();
                }
            }
        }

    }
}
