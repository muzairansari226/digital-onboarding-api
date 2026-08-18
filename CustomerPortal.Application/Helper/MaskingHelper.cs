namespace CustomerPortal.Application.Helper
{
    public static class MaskingHelper
    {
        public static string MaskMobile(string? mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
            {
                return string.Empty;
            }

            var visibleCount = ApplicationConstant.Masking.VisibleMobileDigits;
            if (mobile.Length <= visibleCount)
            {
                return new string(ApplicationConstant.Masking.MaskCharacter, mobile.Length);
            }

            var masked = new string(ApplicationConstant.Masking.MaskCharacter, mobile.Length - visibleCount);
            return string.Concat(masked, mobile.AsSpan(mobile.Length - visibleCount));
        }

        public static string MaskEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return string.Empty;
            }

            var separatorIndex = email.IndexOf('@');
            if (separatorIndex <= 0)
            {
                return new string(ApplicationConstant.Masking.MaskCharacter, email.Length);
            }

            var localPart = email[..separatorIndex];
            var domainPart = email[separatorIndex..];

            if (localPart.Length <= 2)
            {
                return string.Concat(
                    new string(ApplicationConstant.Masking.MaskCharacter, localPart.Length),
                    domainPart);
            }

            var maskedLocal = string.Concat(
                localPart[0],
                new string(ApplicationConstant.Masking.MaskCharacter, localPart.Length - 2),
                localPart[^1]);

            return string.Concat(maskedLocal, domainPart);
        }
    }
}
