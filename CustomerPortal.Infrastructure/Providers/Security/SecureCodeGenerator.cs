using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CustomerPortal.Application.Contracts;

namespace CustomerPortal.Infrastructure.Providers.Security
{
    public class SecureCodeGenerator : ISecureCodeGenerator
    {
        public string GenerateNumericCode(int length)
        {
            // Digit by digit rather than one bounded integer: it keeps the distribution uniform
            // and preserves leading zeros, which a numeric value would silently drop.
            var builder = new StringBuilder(length);

            for (var index = 0; index < length; index++)
            {
                builder.Append(
                    RandomNumberGenerator.GetInt32(0, DigitBase)
                        .ToString(CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        public string GenerateSecret(int byteLength)
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(byteLength))
                .TrimEnd(Base64Padding)
                .Replace(Base64Plus, UrlSafePlus)
                .Replace(Base64Slash, UrlSafeSlash);

        private const int DigitBase = 10;
        private const char Base64Padding = '=';
        private const char Base64Plus = '+';
        private const char Base64Slash = '/';
        private const char UrlSafePlus = '-';
        private const char UrlSafeSlash = '_';
    }
}
