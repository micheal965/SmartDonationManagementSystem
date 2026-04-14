namespace SmartDonationSystem.Core.Modules.Encryption.Interfaces
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText, string purpose);
        string Decrypt(string cipherText, string purpose);
    }
}
