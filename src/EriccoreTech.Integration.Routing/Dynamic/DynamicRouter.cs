using System.Collections.Concurrent;
using System.Text.Json;

namespace EriccoreTech.Integration.Routing {

    // Routes are registered and removed at runtime; the first matching route wins.
    public sealed class DynamicRouter {

        private readonly ConcurrentDictionary<string, Func<JsonElement, bool>> _routes = new(StringComparer.OrdinalIgnoreCase);
        private readonly List<string> _order = [];
        private readonly object _gate = new();

        public void Register(string destination, Func<JsonElement, bool> predicate) {
            lock (_gate) {
                if (_routes.TryAdd(destination, predicate)) {
                    _order.Add(destination);
                } else {
                    _routes[destination] = predicate;
                }
            }
        }

        public bool Unregister(string destination) {
            lock (_gate) {
                _order.RemoveAll(x => string.Equals(x, destination, StringComparison.OrdinalIgnoreCase));
                return _routes.TryRemove(destination, out _);
            }
        }

        public string? Route(JsonElement message) {
            string[] snapshot;
            lock (_gate) {
                snapshot = _order.ToArray();
            }

            foreach (var destination in snapshot) {
                if (_routes.TryGetValue(destination, out var predicate) && predicate(message)) {
                    return destination;
                }
            }

            return null;
        }

    }
}
