using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Web.Models.ZohoESign
{
    public class CreateDocumentResponse
    {
        public int code { get; set; }
        public SignInResponse requests { get; set; }
        public string message { get; set; }
        public string status { get; set; }
    }

    public class ActionData
    {
        public bool verify_recipient { get; set; }
        public string recipient_countrycode_iso { get; set; }
        public string action_type { get; set; }
        public string private_notes { get; set; }
        public string cloud_provider_name { get; set; }
        public string recipient_email { get; set; }
        public bool send_completed_document { get; set; }
        public string verification_type { get; set; }
       // public bool allow_signing { get; set; }
        public string recipient_phonenumber { get; set; }
        public bool is_bulk { get; set; }
        public string action_id { get; set; }
        public int cloud_provider_id { get; set; }
        public int signing_order { get; set; }
        public List<Field> fields { get; set; }
        public string recipient_name { get; set; }
        public string delivery_mode { get; set; }
        
        public string recipient_countrycode { get; set; }

         
                
              
               
    }

    public class DocumentField
    {
        public string document_id { get; set; }
        public List<object> fields { get; set; }
    }

    public class DocumentId
    {
        public string image_string { get; set; }
        public string document_name { get; set; }
        public List<Page> pages { get; set; }
        public int document_size { get; set; }
        public string document_order { get; set; }
        public bool is_editable { get; set; }
        public int total_pages { get; set; }
        public string document_id { get; set; }
    }

    public class Page
    {
        public string image_string { get; set; }
        public int page { get; set; }
        public bool is_thumbnail { get; set; }
    }

    public class SignInResponse
    {
        public string request_status { get; set; }
        public string notes { get; set; }
        public int reminder_period { get; set; }
        public string owner_id { get; set; }
        public string description { get; set; }
        public string request_name { get; set; }
        public long modified_time { get; set; }
        public bool is_deleted { get; set; }
        public int expiration_days { get; set; }
        public bool is_sequential { get; set; }
        public List<object> templates_used { get; set; }
        public string owner_first_name { get; set; }
        public int sign_percentage { get; set; }
        public string owner_email { get; set; }
        public long created_time { get; set; }
        public bool email_reminders { get; set; }
        public List<DocumentId> document_ids { get; set; }
        public bool self_sign { get; set; }
        public List<DocumentField> document_fields { get; set; }
        public string folder_name { get; set; }
        public bool in_process { get; set; }
        public int validity { get; set; }
        public string request_type_name { get; set; }
        public string folder_id { get; set; }
        public string request_id { get; set; }
        public string request_type_id { get; set; }
        public string owner_last_name { get; set; }
        public List<ActionData> actions { get; set; }
    }


    public class Field
    {
        public string field_id { get; set; }
        public int x_coord { get; set; }
        public string field_type_id { get; set; }
        public int abs_height { get; set; }
        public string field_category { get; set; }
        public string field_label { get; set; }
        public bool is_mandatory { get; set; }
        public int page_no { get; set; }
        public string document_id { get; set; }
        public bool is_draggable { get; set; }
        public string field_name { get; set; }
        public double y_value { get; set; }
        public int abs_width { get; set; }
        public string action_id { get; set; }
        public double width { get; set; }
        public int y_coord { get; set; }
        public string field_type_name { get; set; }
        public string description_tooltip { get; set; }
        public double x_value { get; set; }
        public bool is_resizable { get; set; }
        public double height { get; set; }
    }





}
