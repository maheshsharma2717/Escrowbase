using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using SR.EscrowBaseWeb.DataExporting.Excel.NPOI;
using SR.EscrowBaseWeb.E_SignRecords.Dtos;
using SR.EscrowBaseWeb.Dto;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.E_SignRecords.Exporting
{
    public class E_SignRecordsExcelExporter : NpoiExcelExporterBase, IE_SignRecordsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public E_SignRecordsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetE_SignRecordForViewDto> e_SignRecords)
        {
            return CreateExcelPackage(
                "E_SignRecords.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("E_SignRecords"));

                    AddHeader(
                        sheet,
                        L("EmailId"),
                        L("EmbeddedURL"),
                        L("EmbeddedToken"),
                        L("FolderId"),
                        L("FolderName"),
                        L("FolderPassword"),
                        L("PartyId"),
                        L("ContractId"),
                        L("CompanyId"),
                        L("DocumentId")
                        );

                    AddObjects(
                        sheet, 2, e_SignRecords,
                        _ => _.E_SignRecord.EmailId,
                        _ => _.E_SignRecord.EmbeddedURL,
                        _ => _.E_SignRecord.EmbeddedToken,
                        _ => _.E_SignRecord.FolderId,
                        _ => _.E_SignRecord.FolderName,
                        _ => _.E_SignRecord.FolderPassword,
                        _ => _.E_SignRecord.PartyId,
                        _ => _.E_SignRecord.ContractId,
                        _ => _.E_SignRecord.CompanyId,
                        _ => _.E_SignRecord.DocumentId
                        );

                });
        }
    }
}