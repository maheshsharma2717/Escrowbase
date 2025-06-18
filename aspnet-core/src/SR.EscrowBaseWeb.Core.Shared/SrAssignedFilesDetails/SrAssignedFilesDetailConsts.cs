namespace SR.EscrowBaseWeb.SrAssignedFilesDetails
{
    public class SrAssignedFilesDetailConsts
    {

    }
    public class AssignedFileUser
    {
        public string Email { get; set; }
        public string UserType { get; set; }
        public long? UserId { get; set; }
        public long SrEscrowFileMasterId { get; set; }
        public bool IsChecked { get; set; }
        public string FullName { get; set; }
        public string PhoneNo { get; set; }
        public string MessageStatus { get; set; }
        public string EmailStatus { get; set; }
        public string DirectMessageStatus { get; set; }
    }
}