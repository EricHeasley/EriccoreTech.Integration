namespace EriccoreTech.IntegrationFlow.Model {
    public sealed record ListChannelsResponse(IReadOnlyDictionary<string, bool> Channels);
}
