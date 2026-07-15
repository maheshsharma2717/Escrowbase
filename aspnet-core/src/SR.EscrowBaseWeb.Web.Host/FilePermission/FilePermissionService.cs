using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SR.EscrowBaseWeb.EscrowDetails;
using SR.EscrowBaseWeb.SRFileMapping;
using SR.EscrowBaseWeb.SRFileMapping.Dtos;

namespace SR.EscrowBaseWeb.Web.FilePermission
{
    /// <summary>
    /// Grants READ permission to EOX (Escrow Officer) and EOA (Escrow Assistant)
    /// users whenever a file is uploaded or moved to Main/Other sections.
    /// READ = Read, Edit, Alert, Delete, Sign.
    /// </summary>
    public class FilePermissionService : IFilePermissionService
    {
        private readonly IRepository<EscrowDetail, long> _escrowDetailRepository;
        private readonly IRepository<SrFileMapping> _srfilemapRepository;
        private readonly ISrFileMappingsAppService _mappingAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public FilePermissionService(
            IRepository<EscrowDetail, long> escrowDetailRepository,
            IRepository<SrFileMapping> srfilemapRepository,
            ISrFileMappingsAppService mappingAppService,
            IWebHostEnvironment hostingEnvironment)
        {
            _escrowDetailRepository = escrowDetailRepository;
            _srfilemapRepository = srfilemapRepository;
            _mappingAppService = mappingAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <inheritdoc />
        public async Task GrantEoxEoaReadPermissionAsync(string escrowId, long fileMasterId, string filePath, int? excludeUserId = null)
        {
            try
            {
                // 1. Find all EOX and EOA users for this escrow
                var eoxEoaUsers = await _escrowDetailRepository.GetAll()
                    .Where(x => x.EscrowId == escrowId &&
                                (x.Usertype.StartsWith("EO") || x.Usertype.StartsWith("EA")))
                    .ToListAsync();

                if (!eoxEoaUsers.Any())
                    return;

                // 2. For each EOX/EOA user, create a READ mapping if one doesn't already exist
                foreach (var user in eoxEoaUsers)
                {
                    // Skip the uploader if they are already EOX/EOA (they already have a mapping)
                    if (excludeUserId.HasValue && user.UserId == excludeUserId.Value)
                        continue;

                    // Check if a mapping already exists for this user + file
                    var existingMapping = _srfilemapRepository.GetAll()
                        .FirstOrDefault(x => x.UserId == user.UserId &&
                                             x.SrEscrowFileMasterId == fileMasterId);

                    if (existingMapping != null)
                        continue; // Already has permission, skip

                    // Create READ mapping
                    var dto = new CreateOrEditSrFileMappingDto
                    {
                        UserId = (int)user.UserId,
                        Action = "READ",           // READ = Read, Edit, Alert, Delete, Sign
                        EscrowiId = escrowId,
                        IsActive = true,
                        SrEscrowFileMasterId = fileMasterId,
                        FileName = filePath
                    };

                    await _mappingAppService.CreateOrEdit(dto);
                }
            }
            catch (Exception ex)
            {
                // Log but don't throw - permission granting should not break the upload flow
                try
                {
                    string logs = Path.Combine(_hostingEnvironment.WebRootPath, @"Logs\Logs.txt");
                    if (!Directory.Exists(Path.GetDirectoryName(logs)))
                        Directory.CreateDirectory(Path.GetDirectoryName(logs));
                    using StreamWriter writer = new StreamWriter(logs, true);
                    writer.WriteLine($"[{DateTime.Now}] ERROR in GrantEoxEoaReadPermission: escrowId={escrowId}, fileMasterId={fileMasterId} - {ex}");
                }
                catch { }

            }
        }
    }
}
