namespace EriccoreTech.IntegrationFlow.Messaging {
    public sealed record Subscription(Guid Id, string Topic, MessageChannel Channel);
}
