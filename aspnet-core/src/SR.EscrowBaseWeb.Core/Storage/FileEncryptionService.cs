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
        private static readonly byte[] MagicHeaderV1 = Encoding.ASCII.GetBytes("EBENC1"); // 6-byte legacy identifier
        private static readonly string MagicHeaderV2Prefix = "EBENC2|";

        public FileEncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        public byte[] GetEncryptionKey(string escrowNumber = null)
        {
            string keyStr = _configuration?["App:FileEncryption:MasterSecretKey"];

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
                try
                {
                    var fallbackConf = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json", optional: true).Build();
                    keyStr = fallbackConf["App:FileEncryption:MasterSecretKey"];
                }
                catch { }
            }

            if (string.IsNullOrWhiteSpace(keyStr))
            {
                throw new InvalidOperationException("App:FileEncryption:MasterSecretKey is missing from appsettings.json. Please configure a Master Secret Key.");
            }

            if (!string.IsNullOrWhiteSpace(escrowNumber))
            {
                keyStr = keyStr + "_" + escrowNumber.Trim();
            }

            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(keyStr));
            }
        }

        public byte[] EncryptBytes(byte[] plainBytes, string escrowNumber = null)
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
                aes.Key = GetEncryptionKey(escrowNumber);
                aes.GenerateIV();
                byte[] iv = aes.IV;

                byte[] headerBytes;
                if (!string.IsNullOrWhiteSpace(escrowNumber))
                {
                    headerBytes = Encoding.UTF8.GetBytes(MagicHeaderV2Prefix + escrowNumber.Trim() + "|");
                }
                else
                {
                    headerBytes = MagicHeaderV1;
                }

                using (var msOutput = new MemoryStream())
                {
                    // Write Header
                    msOutput.Write(headerBytes, 0, headerBytes.Length);
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

        public byte[] DecryptBytes(byte[] inputBytes, string escrowNumber = null)
        {
            if (inputBytes == null || inputBytes.Length == 0)
            {
                return inputBytes;
            }

            if (!IsEncrypted(inputBytes))
            {
                return inputBytes;
            }

            try
            {
                int headerLen;
                int ivLen = 16;
                string extractedEscrowNumber = escrowNumber;

                if (IsV2Header(inputBytes, out int v2HeaderLength, out string embeddedEscrowNumber))
                {
                    headerLen = v2HeaderLength;
                    if (string.IsNullOrWhiteSpace(extractedEscrowNumber))
                    {
                        extractedEscrowNumber = embeddedEscrowNumber;
                    }
                }
                else
                {
                    headerLen = MagicHeaderV1.Length;
                }

                if (inputBytes.Length < headerLen + ivLen)
                {
                    return inputBytes;
                }

                byte[] iv = new byte[ivLen];
                Buffer.BlockCopy(inputBytes, headerLen, iv, 0, ivLen);

                int cipherTextOffset = headerLen + ivLen;
                int cipherTextLen = inputBytes.Length - cipherTextOffset;

                byte[] key = GetEncryptionKey(extractedEscrowNumber);
                using (var aes = Aes.Create())
                {
                    aes.Key = key;
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
                return inputBytes;
            }
        }

        public async Task EncryptStreamAsync(Stream inputStream, Stream outputStream, string escrowNumber = null)
        {
            using (var ms = new MemoryStream())
            {
                await inputStream.CopyToAsync(ms);
                byte[] encrypted = EncryptBytes(ms.ToArray(), escrowNumber);
                await outputStream.WriteAsync(encrypted, 0, encrypted.Length);
            }
        }

        public async Task DecryptStreamAsync(Stream inputStream, Stream outputStream, string escrowNumber = null)
        {
            using (var ms = new MemoryStream())
            {
                await inputStream.CopyToAsync(ms);
                byte[] decrypted = DecryptBytes(ms.ToArray(), escrowNumber);
                await outputStream.WriteAsync(decrypted, 0, decrypted.Length);
            }
        }

        public async Task EncryptFileAsync(string filePath, string escrowNumber = null)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            if (!IsEncrypted(fileBytes))
            {
                byte[] encryptedBytes = EncryptBytes(fileBytes, escrowNumber);
                await File.WriteAllBytesAsync(filePath, encryptedBytes);
            }
        }

        public async Task<byte[]> ReadAndDecryptFileBytesAsync(string filePath, string escrowNumber = null)
        {
            if (!File.Exists(filePath))
            {
                return Array.Empty<byte>();
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            return DecryptBytes(fileBytes, escrowNumber);
        }

        private bool IsEncrypted(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 6)
            {
                return false;
            }

            if (IsV1Header(bytes))
            {
                return true;
            }

            if (IsV2Header(bytes, out _, out _))
            {
                return true;
            }

            return false;
        }

        private bool IsV1Header(byte[] bytes)
        {
            if (bytes == null || bytes.Length < MagicHeaderV1.Length)
            {
                return false;
            }

            for (int i = 0; i < MagicHeaderV1.Length; i++)
            {
                if (bytes[i] != MagicHeaderV1[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsV2Header(byte[] bytes, out int headerLength, out string escrowNumber)
        {
            headerLength = 0;
            escrowNumber = null;

            if (bytes == null || bytes.Length < MagicHeaderV2Prefix.Length)
            {
                return false;
            }

            string prefix = Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 100));
            if (!prefix.StartsWith(MagicHeaderV2Prefix))
            {
                return false;
            }

            int closingPipe = prefix.IndexOf('|', MagicHeaderV2Prefix.Length);
            if (closingPipe < 0)
            {
                return false;
            }

            escrowNumber = prefix.Substring(MagicHeaderV2Prefix.Length, closingPipe - MagicHeaderV2Prefix.Length);
            headerLength = closingPipe + 1; // Includes starting EBENC2| and ending |
            return true;
        }
    }
}
