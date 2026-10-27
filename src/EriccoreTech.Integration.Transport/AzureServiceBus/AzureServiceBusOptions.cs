namespace EriccoreTech.Integration.Transport {
    public sealed class AzureServiceBusOptions {

        public string ConnectionString { get; set; } = "";

        // Each consumer group needs its own subscription on the topic; it must already exist.
        public string SubscriptionName { get; set; } = "integrationflow";

    }
}
