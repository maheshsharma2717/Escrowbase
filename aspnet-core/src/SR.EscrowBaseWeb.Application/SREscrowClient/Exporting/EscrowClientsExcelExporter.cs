using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using SR.EscrowBaseWeb.DataExporting.Excel.NPOI;
using SR.EscrowBaseWeb.SREscrowClient.Dtos;
using SR.EscrowBaseWeb.Dto;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.SREscrowClient.Exporting
{
    public class EscrowClientsExcelExporter : NpoiExcelExporterBase, IEscrowClientsExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public EscrowClientsExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetEscrowClientForViewDto> escrowClients)
        {
            return CreateExcelPackage(
                "EscrowClients.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("EscrowClients"));

                    AddHeader(
                        sheet,
                        L("EscrowNumber"),
                        L("Name"),
                        L("Email"),
                        L("Phone"),
                        L("Address"),
                        L("State"),
                        L("Pincode")
                        );

                    AddObjects(
                        sheet, 2, escrowClients,
                        _ => _.EscrowClient.EscrowNumber,
                        _ => _.EscrowClient.Name,
                        _ => _.EscrowClient.Email,
                        _ => _.EscrowClient.Phone,
                        _ => _.EscrowClient.Address,
                        _ => _.EscrowClient.State,
                        _ => _.EscrowClient.Pincode
                        );

					
					
                });
        }
    }
}
