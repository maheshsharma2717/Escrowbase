using System.Collections.Generic;
using SR.EscrowBaseWeb.GetCurrentEscrow.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Exporting
{
    public interface ICurrentEscrowsExcelExporter
    {
        FileDto ExportToFile(List<GetCurrentEscrowForViewDto> currentEscrows);
    }
}