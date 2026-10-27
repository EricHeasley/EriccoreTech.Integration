namespace EriccoreTech.IntegrationFlow.Reliability {
    public sealed class DeadLetterPolicy {

        public const string ReadyStatus = "Ready";
        public const string DeadLetterStatus = "DeadLetter";

        public string StatusAfterFailure(RetryPolicy retryPolicy, int attempts) {
            return retryPolicy.IsExhausted(attempts) ? DeadLetterStatus : ReadyStatus;
        }

    }
}
