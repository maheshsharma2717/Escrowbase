using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.EsignRoleMapping.Dtos
{
    public class GetAllEsignRoleMappingsesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}