using System.Text.Json;

namespace EriccoreTech.Integration.Processing {

    // Sends one request to every recipient in parallel and gathers the replies in recipient order.
    public sealed class ScatterGather {

        public async Task<IReadOnlyList<JsonElement>> ExecuteAsync(
            JsonElement request,
            IReadOnlyList<Func<JsonElement, CancellationToken, Task<JsonElement>>> recipients,
            CancellationToken cancellationToken = default) {
            var replies = await Task.WhenAll(recipients.Select(recipient => recipient(request, cancellationToken)));
            return replies.Select(x => x.Clone()).ToArray();
        }

    }
}
