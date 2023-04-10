using System.ComponentModel.DataAnnotations;

namespace PersonalSiteConversion.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "* Name is required")] 
        public string Name { get; set; } = null!;


        [Required(ErrorMessage = "* Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "* Message is required")]
        [DataType(DataType.MultilineText)] 
        public string Message { get; set; } = null!;
    }
}
