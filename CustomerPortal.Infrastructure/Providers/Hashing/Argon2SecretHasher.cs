using System.Security.Cryptography;
using System.Text;
using CustomerPortal.Application.Contracts;
using CustomerPortal.Application.Settings;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace CustomerPortal.Infrastructure.Providers.Hashing
{
    public class Argon2SecretHasher : ISecretHasher
    {
        private readonly HashingSettings _hashingSettings;

        public Argon2SecretHasher(IOptions<HashingSettings> hashingSettings)
        {
            _hashingSettings = hashingSettings.Value;
        }

        public string GenerateSalt()
            => Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(_hashingSettings.SaltLengthBytes));

        public string Hash(string plainText, string salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(plainText))
            {
                Salt = Convert.FromBase64String(salt),
                Iterations = _hashingSettings.Iterations,
                MemorySize = _hashingSettings.MemorySizeKb,
                DegreeOfParallelism = _hashingSettings.DegreeOfParallelism
            };

            return Convert.ToBase64String(argon2.GetBytes(_hashingSettings.HashLengthBytes));
        }

        public bool Verify(string plainText, string salt, string expectedHash)
        {
            var actualBytes = Convert.FromBase64String(Hash(plainText, salt));
            var expectedBytes = Convert.FromBase64String(expectedHash);

            // Fixed-time comparison: an ordinary equality check leaks how many bytes matched
            // through how long it took to fail.
            return CryptographicOperations.FixedTimeEquals(actualBytes, expectedBytes);
        }
    }
}
