using System.Collections.Concurrent;

namespace EriccoreTech.IntegrationFlow.Orchestration {

    // Tracks the status of long-running processes; every change goes through the state machine.
    public sealed class ProcessManager {

        private readonly ConcurrentDictionary<Guid, ProcessStatus> _processes = new();

        public Guid Start() {
            var id = Guid.NewGuid();
            _processes[id] = ProcessStatus.Created;
            return id;
        }

        public ProcessStatus Advance(Guid id, ProcessStatus next) {
            if (!_processes.TryGetValue(id, out var current)) {
                throw new KeyNotFoundException($"Unknown process: {id}");
            }

            var updated = ProcessStateMachine.Transition(current, next);
            _processes[id] = updated;
            return updated;
        }

        public ProcessStatus? Get(Guid id) {
            return _processes.TryGetValue(id, out var status) ? status : null;
        }

    }
}
