using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using SR.EscrowBaseWeb.DataExporting.Excel.NPOI;
using SR.EscrowBaseWeb.SREnterprise.Dtos;
using SR.EscrowBaseWeb.Dto;
using SR.EscrowBaseWeb.Storage;

namespace SR.EscrowBaseWeb.SREnterprise.Exporting
{
    public class EnterprisesExcelExporter : NpoiExcelExporterBase, IEnterprisesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public EnterprisesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
			ITempFileCacheManager tempFileCacheManager) :  
	base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetEnterpriseForViewDto> enterprises)
        {
            return CreateExcelPackage(
                "Enterprises.xlsx",
                excelPackage =>
                {
                    
                    var sheet = excelPackage.CreateSheet(L("Enterprises"));

                    AddHeader(
                        sheet,
                        L("EnterpriseName"),
                        L("Email"),
                        L("Phone"),
                        L("Address1"),
                        L("Address2"),
                        L("City"),
                        L("State"),
                        L("PinCode"),
                        L("Contry")
                        );

                    AddObjects(
                        sheet, 2, enterprises,
                        _ => _.Enterprise.EnterpriseName,
                        _ => _.Enterprise.Email,
                        _ => _.Enterprise.Phone,
                        _ => _.Enterprise.Address1,
                        _ => _.Enterprise.Address2,
                        _ => _.Enterprise.City,
                        _ => _.Enterprise.State,
                        _ => _.Enterprise.PinCode,
                        _ => _.Enterprise.Contry
                        );

					
					
                });
        }
    }
}
