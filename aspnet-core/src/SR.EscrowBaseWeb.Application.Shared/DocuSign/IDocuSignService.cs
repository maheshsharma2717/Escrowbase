using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SR.EscrowBaseWeb.DocuSign.Dtos;

namespace SR.EscrowBaseWeb.DocuSign
{
    public interface IDocuSignService
    {
        Task<SendEnvelopeResultDto> SendEnvelopeAsync(CreateOrEditDocuSignDto input);
    }
}
