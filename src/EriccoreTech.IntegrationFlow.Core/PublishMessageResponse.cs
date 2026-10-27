namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record PublishMessageResponse(Guid MessageId, Guid CorrelationId);
}
