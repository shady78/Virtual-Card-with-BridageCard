namespace PayLi.API.Services
{
    public interface IEncryptionService
    {
        string EncryptPin(string pin, string secretKey);
        string DecryptPin(string encryptedPin, string secretKey);
    }
}
