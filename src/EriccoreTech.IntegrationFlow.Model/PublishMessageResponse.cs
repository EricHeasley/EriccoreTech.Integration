namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record PublishMessageResponse(Guid MessageId, Guid CorrelationId);
}
