using System.ComponentModel.DataAnnotations;

namespace BankAccounts.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "is required")]
        [EmailAddress]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "is required")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}