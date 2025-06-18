using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Web.Models.ZohoESign
{
    public class ZohoSignatureStatus
    {
        public int code { get; set; }
        public DocumentFormData document_form_data { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }

    public class ActionDataList
    {
        public string signed_time { get; set; }
        public string action_type { get; set; }
        public string recipient_email { get; set; }
        public string recipient_name { get; set; }
        public List<FieldList> fields { get; set; }
    }

    public class DocumentFieldList
    {
        public string document_name { get; set; }
        public List<object> fields { get; set; }
    }

    public class DocumentFormData
    {
        public string request_name { get; set; }
        public List<DocumentFieldList> document_fields { get; set; }
        public string zsdocumentid { get; set; }
        public List<ActionDataList> actions { get; set; }
    }

    public class FieldList
    {
        public string field_label { get; set; }
        public string field_value { get; set; }
        public string field_name { get; set; }
    }


}
