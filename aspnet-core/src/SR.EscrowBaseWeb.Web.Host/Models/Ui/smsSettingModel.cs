using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace SR.EscrowBaseWeb.Web.Models.Ui
{
    public class smsSettingModel
    {
        public bool Ichecked { get; set; }

        public int userid { get; set; }

        public string function { get; set; }


    }
}
