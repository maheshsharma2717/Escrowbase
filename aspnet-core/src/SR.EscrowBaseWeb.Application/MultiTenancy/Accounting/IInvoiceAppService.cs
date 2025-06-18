using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.MultiTenancy.Accounting.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
