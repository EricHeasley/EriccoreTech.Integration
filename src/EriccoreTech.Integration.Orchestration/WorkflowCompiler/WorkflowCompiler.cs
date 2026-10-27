using EriccoreTech.Integration.Core;

namespace EriccoreTech.Integration.Orchestration {
    public static class WorkflowCompiler {

        public static IReadOnlyList<StepDefinition> Compile(WorkflowDefinition definition) {
            if (string.IsNullOrWhiteSpace(definition.Id)) {
                throw new ArgumentException("Workflow ID is required.");
            }

            if (definition.Steps is null) {
                throw new ArgumentException("Steps are required.");
            }

            var map = new Dictionary<string, StepDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (var step in definition.Steps) {
                if (string.IsNullOrWhiteSpace(step.Id) || string.IsNullOrWhiteSpace(step.Component)) {
                    throw new ArgumentException("Step ID and component are required.");
                }

                if (step.MaxAttempts < 1 || step.TimeoutSeconds < 1) {
                    throw new ArgumentException($"Invalid execution policy on {step.Id}");
                }

                if (!map.TryAdd(step.Id, step)) {
                    throw new ArgumentException($"Duplicate step: {step.Id}");
                }
            }

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var visiting = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var ordered = new List<StepDefinition>();

            void Visit(string id) {
                if (!map.TryGetValue(id, out var step)) {
                    throw new ArgumentException($"Unknown dependency: {id}");
                }

                if (visited.Contains(id)) {
                    return;
                }

                if (!visiting.Add(id)) {
                    throw new ArgumentException($"Dependency cycle at: {id}");
                }

                foreach (var dependency in step.DependsOn ?? []) {
                    Visit(dependency);
                }

                visiting.Remove(id);
                visited.Add(id);
                ordered.Add(step);
            }

            foreach (var step in definition.Steps) {
                Visit(step.Id);
            }

            return ordered;
        }

    }
}
