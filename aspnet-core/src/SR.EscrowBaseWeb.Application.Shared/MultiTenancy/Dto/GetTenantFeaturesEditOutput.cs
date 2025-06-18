using System.Collections.Generic;
using Abp.Application.Services.Dto;
using SR.EscrowBaseWeb.Editions.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}