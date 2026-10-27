namespace EriccoreTech.Integration.Core {
    public sealed record PublishMessageResponse(Guid MessageId, Guid CorrelationId);
}
