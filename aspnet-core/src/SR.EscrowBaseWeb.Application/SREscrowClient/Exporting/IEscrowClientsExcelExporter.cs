using System.Collections.Generic;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.SREscrowClient.Exporting
{
    public interface IEscrowClientsExcelExporter
    {
        FileDto ExportToFile(List<GetEscrowClientForViewDto> escrowClients);
    }
}