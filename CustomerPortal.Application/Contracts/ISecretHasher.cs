namespace CustomerPortal.Application.Contracts
{
    public interface ISecretHasher
    {
        string GenerateSalt();
        string Hash(string plainText, string salt);
        bool Verify(string plainText, string salt, string expectedHash);
    }
}
