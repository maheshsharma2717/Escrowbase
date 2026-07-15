using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Web.FilePermission
{
    /// <summary>
    /// Grants READ permission (Read, Edit, Alert, Delete, Sign) to the
    /// EOX and EOA users for a given file when it's uploaded.
    /// </summary>
    public interface IFilePermissionService
    {
        /// <summary>
        /// Adds READ mappings for EOX and EOA users of the given escrow.
        /// Skips users that already have a mapping for the same file.
        /// </summary>
        /// <param name="escrowId">Escrow identifier (string)</param>
        /// <param name="fileMasterId">Id of the SrEscrowFileMaster record</param>
        /// <param name="filePath">Full file path for the mapping record</param>
        /// <param name="excludeUserId">UserId to exclude (e.g. the uploader who already has a mapping)</param>
        Task GrantEoxEoaReadPermissionAsync(string escrowId, long fileMasterId, string filePath, int? excludeUserId = null);
    }
}
