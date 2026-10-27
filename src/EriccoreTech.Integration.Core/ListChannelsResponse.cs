namespace EriccoreTech.Integration.Core {
    public sealed record ListChannelsResponse(IReadOnlyDictionary<string, bool> Channels);
}
