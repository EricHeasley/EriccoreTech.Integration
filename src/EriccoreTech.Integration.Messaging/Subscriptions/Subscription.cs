namespace EriccoreTech.Integration.Messaging {
    public sealed record Subscription(Guid Id, string Topic, MessageChannel Channel);
}
