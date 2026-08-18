namespace CustomerPortal.Application.Helper
{
    public static class PinStrengthHelper
    {
        public static bool IsTrivial(string pin)
        {
            if (string.IsNullOrWhiteSpace(pin))
            {
                return true;
            }

            if (pin.All(digit => digit == pin[0]))
            {
                return true;
            }

            return ApplicationConstant.PinRules.AscendingDigits.Contains(pin, StringComparison.Ordinal)
                || ApplicationConstant.PinRules.DescendingDigits.Contains(pin, StringComparison.Ordinal);
        }
    }
}
