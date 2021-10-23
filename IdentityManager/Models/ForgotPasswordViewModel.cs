using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
