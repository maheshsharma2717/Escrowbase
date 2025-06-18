using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SR.EscrowBaseWeb.MultiTenancy.HostDashboard.Dto;

namespace SR.EscrowBaseWeb.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}