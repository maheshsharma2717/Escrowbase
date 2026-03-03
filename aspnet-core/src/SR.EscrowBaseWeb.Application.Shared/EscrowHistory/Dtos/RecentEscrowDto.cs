using System;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class RecentEscrowDto
    {
        public int EscrowId { get; set; }
        public string EscrowNumber { get; set; }
        public string CompanyName { get; set; }
        public string SubCompanyName { get; set; }
        public string UserType { get; set; }
        public DateTime LastAccessTime { get; set; }
    }
}
