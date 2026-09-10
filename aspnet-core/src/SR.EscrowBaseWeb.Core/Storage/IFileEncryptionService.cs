using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Dependency;

namespace SR.EscrowBaseWeb.Storage
{
    public interface IFileEncryptionService : ISingletonDependency
    {
        byte[] EncryptBytes(byte[] plainBytes);
        byte[] DecryptBytes(byte[] inputBytes);
        Task EncryptStreamAsync(Stream inputStream, Stream outputStream);
        Task DecryptStreamAsync(Stream inputStream, Stream outputStream);
        Task EncryptFileAsync(string filePath);
        Task<byte[]> ReadAndDecryptFileBytesAsync(string filePath);
    }
}
