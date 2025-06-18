using System;

namespace SR.EscrowBaseWeb.Sessions.Dto
{
    public class UpdateUserSignInTokenOutput
    {
        public string SignInToken { get; set; }

        public string EncodedUserId { get; set; }

        public string EncodedTenantId { get; set; }
    }

    public class zohoEsignAccessToken
    {

        public string zohoAccessToken { get; set; }
        public DateTime accessTokenTime { get; set; }
    }
}