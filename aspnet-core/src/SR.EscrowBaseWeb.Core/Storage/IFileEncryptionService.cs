using System;
using System.IO;
using System.Threading.Tasks;
using Abp.Dependency;

namespace SR.EscrowBaseWeb.Storage
{
    public interface IFileEncryptionService : ISingletonDependency
    {
        byte[] EncryptBytes(byte[] plainBytes, string escrowNumber = null);
        byte[] DecryptBytes(byte[] inputBytes, string escrowNumber = null);
        Task EncryptStreamAsync(Stream inputStream, Stream outputStream, string escrowNumber = null);
        Task DecryptStreamAsync(Stream inputStream, Stream outputStream, string escrowNumber = null);
        Task EncryptFileAsync(string filePath, string escrowNumber = null);
        Task<byte[]> ReadAndDecryptFileBytesAsync(string filePath, string escrowNumber = null);
    }
}
