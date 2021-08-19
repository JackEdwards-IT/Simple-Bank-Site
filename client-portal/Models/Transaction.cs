using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.Models
{
    public static class TransactionType
    {
        public const char Deposit = 'D';
        public const char Withdraw = 'W';
        public const char Transfer = 'T';
        public const char ServiceCharge = 'S';
        public const char BillPay = 'B';
    }

    public class Transaction
    {
        [Required]
        public int TransactionID { get; set; }
        
        [Required, Column(TypeName = "char")]
        public char TransactionType { get; set; }

        [Required, ForeignKey("Account")]
        public int AccountNumber { get; set; }
        public virtual Account Account { get; set; }

        [ForeignKey("DestinationAccount")]
        public int? DestinationAccountNumber { get; set; }
        public virtual Account DestinationAccount { get; set; }

        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [StringLength(30)]
        public string Comment { get; set; }

        [Required]
        public DateTime TransactionTimeUtc { get; set; }
    }
}
