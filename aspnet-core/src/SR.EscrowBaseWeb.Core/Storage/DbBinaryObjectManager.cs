using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SR.EscrowBaseWeb.Storage
{
    public class DbBinaryObjectManager : IBinaryObjectManager, ITransientDependency
    {
        private readonly IRepository<BinaryObject, Guid> _binaryObjectRepository;
        private readonly IFileEncryptionService _fileEncryptionService;

        public DbBinaryObjectManager(
            IRepository<BinaryObject, Guid> binaryObjectRepository,
            IFileEncryptionService fileEncryptionService)
        {
            _binaryObjectRepository = binaryObjectRepository;
            _fileEncryptionService = fileEncryptionService;
        }

        public async Task<BinaryObject> GetOrNullAsync(Guid id)
        {
            var item = await _binaryObjectRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (item != null && item.Bytes != null)
            {
                item.Bytes = _fileEncryptionService.DecryptBytes(item.Bytes);
            }
            return item;
        }

        public async Task SaveAsync(BinaryObject file)
        {
            if (file != null && file.Bytes != null)
            {
                file.Bytes = _fileEncryptionService.EncryptBytes(file.Bytes);
            }
            await _binaryObjectRepository.InsertAsync(file);
        }

        public Task DeleteAsync(Guid id)
        {
            return _binaryObjectRepository.DeleteAsync(id);
        }
    }
}