namespace CustomerPortal.Application.Settings
{
    public class DatabaseSettings
    {
        public int MaxRetryCount { get; set; }

        public int MaxRetryDelaySeconds { get; set; }

        public int CommandTimeoutSeconds { get; set; }
    }
}
