using System.Collections.Generic;
using SR.EscrowBaseWeb.SREnterprise.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.SREnterprise.Exporting
{
    public interface IEnterprisesExcelExporter
    {
        FileDto ExportToFile(List<GetEnterpriseForViewDto> enterprises);
    }
}