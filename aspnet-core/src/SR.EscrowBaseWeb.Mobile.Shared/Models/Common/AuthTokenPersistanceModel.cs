using System;
using Abp.AutoMapper;
using SR.EscrowBaseWeb.Sessions.Dto;

namespace SR.EscrowBaseWeb.Models.Common
{
    [AutoMapFrom(typeof(ApplicationInfoDto)),
     AutoMapTo(typeof(ApplicationInfoDto))]
    public class ApplicationInfoPersistanceModel
    {
        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }
    }
}