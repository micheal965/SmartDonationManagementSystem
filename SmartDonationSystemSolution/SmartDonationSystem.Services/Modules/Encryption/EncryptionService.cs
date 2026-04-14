using Microsoft.AspNetCore.DataProtection;
using SmartDonationSystem.Core.Modules.Encryption.Interfaces;

namespace SmartDonationSystem.Services.Modules.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtectionProvider _provider;
        public EncryptionService(IDataProtectionProvider provider)
        {
            _provider = provider;
        }

        public string Encrypt(string plainText, string purpose)
        {
            var protector = _provider.CreateProtector(purpose);
            return protector.Protect(plainText);
        }

        public string Decrypt(string cipherText, string purpose)
        {
            var protector = _provider.CreateProtector(purpose);
            return protector.Unprotect(cipherText);
        }
    }
}
