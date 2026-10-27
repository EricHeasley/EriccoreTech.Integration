namespace EriccoreTech.IntegrationFlow.Core {
    public sealed record ListChannelsResponse(IReadOnlyDictionary<string, bool> Channels);
}
