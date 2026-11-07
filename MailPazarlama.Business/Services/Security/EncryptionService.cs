using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MailPazarlama.Business.Services.Security;

public class EncryptionService : IEncryptionService
{
    private const string DefaultKey = "TRtek_MailPazarlama_Secret_2026!";
    private const string DefaultIV = "1234567890123456";

    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionService(IConfiguration configuration)
    {
        var keyString = configuration["Security:EncryptionKey"] ?? DefaultKey;
        var ivString = configuration["Security:EncryptionIV"] ?? DefaultIV;

        // Devglan ve online AES araçlarıyla (AES-256-CBC) birebir uyum için doğrudan UTF8 baytları kullanılır
        _key = Encoding.UTF8.GetBytes(keyString.PadRight(32).Substring(0, 32));
        _iv = Encoding.UTF8.GetBytes(ivString.PadRight(16).Substring(0, 16));
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _key;
        aes.IV = _iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        var cipherBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _key;
        aes.IV = _iv;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(cipherBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);

        return sr.ReadToEnd();
    }
}
