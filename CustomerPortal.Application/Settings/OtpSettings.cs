namespace CustomerPortal.Application.Settings
{
    public class OtpSettings
    {
        public int CodeLength { get; set; }

        public int ExpiryMinutes { get; set; }

        public int MaxVerificationAttempts { get; set; }

        public int ResendCooldownSeconds { get; set; }

        public int MaxResendsPerWindow { get; set; }

        public int ResendWindowMinutes { get; set; }

        public bool ReturnCodeInResponse { get; set; }
    }
}
