using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.admin.Models
{
    public static class Period
    {
        public const char Monthly = 'M';
        public const char Quarterly = 'Q';
        public const char Annually = 'A';
        public const char OneOff = 'O';
    }

    public class BillPay
    {
        public int BillPayID {get; set;}

        [Display(Name = "Account Number")]
        [ForeignKey("Account")]

        [Required]
        // FK
        public int AccountNumber { get; set; }
        public virtual Account Account {get; set;}

        [Required]
        public int PayeeID { get; set; }

        [Column(TypeName = "money")]
        [DataType(DataType.Currency), Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ScheduledTimeUtc { get; set; }

        [Required, Column(TypeName = "char")]
        public Char Period {get; set; }
        public Boolean Failed {get; set; }
        public Boolean PaymentLocked {get; set;}
    }
}