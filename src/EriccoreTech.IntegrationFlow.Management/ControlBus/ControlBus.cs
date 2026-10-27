using System.Collections.Concurrent;

namespace EriccoreTech.IntegrationFlow.Management {
    public sealed class ControlBus {

        private readonly ConcurrentDictionary<string, bool> _paused = new(StringComparer.OrdinalIgnoreCase);

        public void Pause(string topic) {
            _paused[topic] = true;
        }

        public void Resume(string topic) {
            _paused[topic] = false;
        }

        public bool IsPaused(string topic) {
            return _paused.TryGetValue(topic, out var paused) && paused;
        }

        public IReadOnlyDictionary<string, bool> Snapshot() {
            return new Dictionary<string, bool>(_paused);
        }

    }
}
