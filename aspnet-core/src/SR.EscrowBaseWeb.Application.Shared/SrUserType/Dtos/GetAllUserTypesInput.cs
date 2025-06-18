using Abp.Application.Services.Dto;
using System;

namespace SR.EscrowBaseWeb.SrUserType.Dtos
{
    public class GetAllUserTypesInput : PagedAndSortedResultRequestDto
    {
		public string Filter { get; set; }

		public string TypeFilter { get; set; }



    }
}