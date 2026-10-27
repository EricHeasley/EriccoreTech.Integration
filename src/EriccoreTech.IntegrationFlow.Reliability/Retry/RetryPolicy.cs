namespace EriccoreTech.IntegrationFlow.Reliability {

    // Exponential backoff: BaseDelay * 2^(attempt - 1), capped at MaxDelay.
    public sealed record RetryPolicy(int MaxAttempts, TimeSpan BaseDelay, TimeSpan MaxDelay) {

        public static RetryPolicy Default { get; } = new(5, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(300));

        public TimeSpan DelayFor(int attempt) {
            var exponent = Math.Clamp(attempt - 1, 0, 30);
            var seconds = Math.Min(MaxDelay.TotalSeconds, BaseDelay.TotalSeconds * Math.Pow(2, exponent));
            return TimeSpan.FromSeconds(seconds);
        }

        public bool IsExhausted(int attempts) {
            return attempts >= MaxAttempts;
        }

    }
}
