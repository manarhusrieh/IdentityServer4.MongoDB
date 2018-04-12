namespace IdentityServer4.MongoDB.Options
{
    public class TokenCleanupOptions
    {
        public bool EnableTokenCleanup { get; set; } = false;

        public int Interval { get; set; } = 3600;

        public int TokenCleanupBatchSize { get; set; } = 100;
    }
}