namespace CustomerPortal.Application.Settings
{
    public class RateLimitSettings
    {
        public RateLimitPolicySettings OtpSend { get; set; } = new();

        public RateLimitPolicySettings OtpVerify { get; set; } = new();

        public RateLimitPolicySettings PinVerify { get; set; } = new();

        public RateLimitPolicySettings RegistrationStart { get; set; } = new();
    }

    public class RateLimitPolicySettings
    {
        public int PermitLimit { get; set; }

        public int WindowSeconds { get; set; }
    }
}
