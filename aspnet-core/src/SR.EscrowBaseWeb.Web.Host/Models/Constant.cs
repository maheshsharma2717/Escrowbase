using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SR.EscrowBaseWeb.Web.Models
{
    public static class FileConstant
    {
        public  const string ADD_File = "File added successfully to Escrow.";

        public const string Sign_File = "File sent for electronic signature.";

        public const string Sign_File_Download = "Signed file updated successfully.";

        public const string Download_File = "File downloaded successfully.";
        public const string Edit_File = "File edited successfully.";

        public const string Rename_File = "File renamed successfully.";
        public const string Reminder_Message = "Reminder mail has been sent.";

    }

    public class FileConstantAction
    {
        public const string ADD_File = "Add File";

        public const string Sign_File = "E-Sign";

        public const string Download_File = "Download File";

        public const string Edit_File = "Edit File";
        public const string Rename_File = "Rename File";
        public const string Reminder_Message = "Add New Reminder";

    }
}
