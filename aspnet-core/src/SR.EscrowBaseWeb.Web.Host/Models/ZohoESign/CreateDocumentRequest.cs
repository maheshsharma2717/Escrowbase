using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Web.Models.ZohoESign
{
    public class CreateDocumentRequest
    {
        public Requests requests { get; set; }

    }

    public class Action
    {
        public string action_type { get; set; }
        public string recipient_email { get; set; }
        public string recipient_name { get; set; }
        public object signing_order { get; set; }
        public object verify_recipient { get; set; }
        public string verification_type { get; set; }
        public string verification_code { get; set; }
        public string private_notes { get; set; }
        public bool is_embedded { get; set; }
        public bool is_bulk { get; set; }



    }

    public class Requests
    {
        public string request_name { get; set; }
        public List<Action> actions { get; set; }
        public object expiration_days { get; set; }
        public object is_sequential { get; set; }
        public object email_reminders { get; set; }
        public object reminder_period { get; set; }
        public string folder_id { get; set; }
    }




    public class ZohotokenApiResponse
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string api_domain { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

}
