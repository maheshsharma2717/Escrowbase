using System.Collections.Generic;
using SR.EscrowBaseWeb.E_SignRecords.Dtos;
using SR.EscrowBaseWeb.Dto;

namespace SR.EscrowBaseWeb.E_SignRecords.Exporting
{
    public interface IE_SignRecordsExcelExporter
    {
        FileDto ExportToFile(List<GetE_SignRecordForViewDto> e_SignRecords);
    }
}