namespace SR.EscrowBaseWeb.EscrowFileReminder.Dtos
{
    public class GetSrEscrowFileReminderForViewDto
    {
        public SrEscrowFileReminderDto SrEscrowFileReminder { get; set; }

        public string SREscrowFileMasterFileFullName { get; set; }

        public string UserName { get; set; }

    }
}