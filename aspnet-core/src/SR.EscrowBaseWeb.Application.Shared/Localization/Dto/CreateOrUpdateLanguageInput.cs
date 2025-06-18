using System.ComponentModel.DataAnnotations;

namespace SR.EscrowBaseWeb.Localization.Dto
{
    public class CreateOrUpdateLanguageInput
    {
        [Required]
        public ApplicationLanguageEditDto Language { get; set; }
    }
}