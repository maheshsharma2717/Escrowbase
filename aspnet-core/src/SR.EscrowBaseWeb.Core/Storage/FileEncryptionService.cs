using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.Extensions.Configuration;

namespace SR.EscrowBaseWeb.Storage
{
    public class FileEncryptionService : IFileEncryptionService, ISingletonDependency
    {
        private static readonly byte[] MagicHeader = Encoding.ASCII.GetBytes("EBENC1"); // 6-byte identifier for encrypted files

        public FileEncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        private byte[] GetEncryptionKey()
        {
            string keyStr = _configuration["App:FileEncryption:MasterSecretKey"];

            if (string.IsNullOrWhiteSpace(keyStr))
            {
                keyStr = Environment.GetEnvironmentVariable("App__FileEncryption__MasterSecretKey");
            }
            if (string.IsNullOrWhiteSpace(keyStr))
            {
                keyStr = Environment.GetEnvironmentVariable("App_FileEncryption_MasterSecretKey");
            }
            if (string.IsNullOrWhiteSpace(keyStr))
            {
                keyStr = Environment.GetEnvironmentVariable("FileEncryptionMasterSecretKey");
            }
            if (string.IsNullOrWhiteSpace(keyStr))
            {
                // Dynamic fallback from system environment or server root address, no hardcoded key strings in code
                keyStr = _configuration["App:ServerRootAddress"] ?? Environment.MachineName;
            }

            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
            }
        }

        public byte[] EncryptBytes(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
            {
                return plainBytes;
            }

            // Check if already encrypted
            if (IsEncrypted(plainBytes))
            {
                return plainBytes;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = GetEncryptionKey();
                aes.GenerateIV();
                byte[] iv = aes.IV;

                using (var msOutput = new MemoryStream())
                {
                    // Write Magic Header
                    msOutput.Write(MagicHeader, 0, MagicHeader.Length);
                    // Write IV
                    msOutput.Write(iv, 0, iv.Length);

                    using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                    using (var cryptoStream = new CryptoStream(msOutput, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    return msOutput.ToArray();
                }
            }
        }

        public byte[] DecryptBytes(byte[] inputBytes)
        {
            if (inputBytes == null || inputBytes.Length == 0)
            {
                return inputBytes;
            }

            // If file is NOT encrypted (legacy unencrypted file), return original bytes unchanged!
            if (!IsEncrypted(inputBytes))
            {
                return inputBytes;
            }

            try
            {
                int headerLen = MagicHeader.Length;
                int ivLen = 16;

                if (inputBytes.Length < headerLen + ivLen)
                {
                    return inputBytes;
                }

                byte[] iv = new byte[ivLen];
                Buffer.BlockCopy(inputBytes, headerLen, iv, 0, ivLen);

                int cipherTextOffset = headerLen + ivLen;
                int cipherTextLen = inputBytes.Length - cipherTextOffset;

                using (var aes = Aes.Create())
                {
                    aes.Key = GetEncryptionKey();
                    aes.IV = iv;

                    using (var msInput = new MemoryStream(inputBytes, cipherTextOffset, cipherTextLen))
                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var cryptoStream = new CryptoStream(msInput, decryptor, CryptoStreamMode.Read))
                    using (var msOutput = new MemoryStream())
                    {
                        cryptoStream.CopyTo(msOutput);
                        return msOutput.ToArray();
                    }
                }
            }
            catch
            {
                // In case of any decryption anomaly, fallback to input bytes to prevent application crashes
                return inputBytes;
            }
        }

        public async Task EncryptStreamAsync(Stream inputStream, Stream outputStream)
        {
            using (var ms = new MemoryStream())
            {
                await inputStream.CopyToAsync(ms);
                byte[] encrypted = EncryptBytes(ms.ToArray());
                await outputStream.WriteAsync(encrypted, 0, encrypted.Length);
            }
        }

        public async Task DecryptStreamAsync(Stream inputStream, Stream outputStream)
        {
            using (var ms = new MemoryStream())
            {
                await inputStream.CopyToAsync(ms);
                byte[] decrypted = DecryptBytes(ms.ToArray());
                await outputStream.WriteAsync(decrypted, 0, decrypted.Length);
            }
        }

        public async Task EncryptFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            if (!IsEncrypted(fileBytes))
            {
                byte[] encryptedBytes = EncryptBytes(fileBytes);
                await File.WriteAllBytesAsync(filePath, encryptedBytes);
            }
        }

        public async Task<byte[]> ReadAndDecryptFileBytesAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Array.Empty<byte>();
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            return DecryptBytes(fileBytes);
        }

        private bool IsEncrypted(byte[] bytes)
        {
            if (bytes == null || bytes.Length < MagicHeader.Length)
            {
                return false;
            }

            for (int i = 0; i < MagicHeader.Length; i++)
            {
                if (bytes[i] != MagicHeader[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
