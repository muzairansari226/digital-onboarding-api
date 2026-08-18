namespace CustomerPortal.Application.Settings
{
    public class PinSettings
    {
        public int Length { get; set; }

        public int MaxFailedAttempts { get; set; }

        public int LockoutMinutes { get; set; }
    }
}
