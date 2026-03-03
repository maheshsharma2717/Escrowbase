using System;

namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class LogAccessInput
    {
        public int? EscrowId { get; set; }
        public string EscrowNumber { get; set; }
    }
}
