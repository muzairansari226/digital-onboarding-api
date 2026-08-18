namespace CustomerPortal.Application.Contracts
{
    public interface ISecureCodeGenerator
    {
        string GenerateNumericCode(int length);
        string GenerateSecret(int byteLength);
    }
}
