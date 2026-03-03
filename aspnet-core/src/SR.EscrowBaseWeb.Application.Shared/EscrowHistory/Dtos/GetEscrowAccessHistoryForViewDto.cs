namespace SR.EscrowBaseWeb.EscrowHistory.Dtos
{
    public class GetEscrowAccessHistoryForViewDto
    {
        public EscrowAccessHistoryDto EscrowAccessHistory { get; set; }

        public string UserName { get; set; }

        public string EscrowClientName { get; set; }

    }
}