using System.Collections.Generic;
using SR.EscrowBaseWeb.Auditing.Dto;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
