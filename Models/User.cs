using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccounts.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set;}

        [Required(ErrorMessage = "is required")]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "is required")]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "is required")]
        [Compare("Password",ErrorMessage = "Passwords must match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<Transaction> Transactions { get; set; }
    }
}