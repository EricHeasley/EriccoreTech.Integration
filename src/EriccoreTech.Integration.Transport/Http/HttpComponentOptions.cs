namespace EriccoreTech.Integration.Transport {
    public sealed class HttpComponentOptions {

        // Hosts the http component may call. Empty means every call is refused.
        public string[] AllowedHosts { get; set; } = Array.Empty<string>();

        public int TimeoutSeconds { get; set; } = 30;

    }
}
