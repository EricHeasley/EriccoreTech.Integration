namespace EriccoreTech.IntegrationFlow.Routing {
    public sealed class RoutingSlip {

        public IReadOnlyList<string> Next(IReadOnlyList<string> steps, int cursor) {
            return steps.Skip(Math.Max(0, cursor)).ToArray();
        }

    }
}
