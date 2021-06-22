using System;
using System.ComponentModel.DataAnnotations;

namespace BankAccounts.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        [Required(ErrorMessage ="is required")]
        [Display(Name ="Withdraw/Deposit: ")]
        public Decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public User Owner { get; set; }
    }
}