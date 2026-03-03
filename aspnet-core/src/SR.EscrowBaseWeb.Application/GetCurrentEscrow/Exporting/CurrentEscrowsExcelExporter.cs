using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using SR.EscrowBaseWeb.DataExporting.Excel.NPOI;
using SR.EscrowBaseWeb.GetCurrentEscrow.Dtos;
using SR.EscrowBaseWeb.Dto;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.GetCurrentEscrow.Exporting
{
    public class CurrentEscrowsExcelExporter : NpoiExcelExporterBase, ICurrentEscrowsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public CurrentEscrowsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetCurrentEscrowForViewDto> currentEscrows)
        {
            return CreateExcelPackage(
                "CurrentEscrows.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("CurrentEscrows"));

                    AddHeader(
                        sheet
                        );

                    //AddObjects(
                    //    sheet, currentEscrows
                    //    );

                });
        }
    }
}