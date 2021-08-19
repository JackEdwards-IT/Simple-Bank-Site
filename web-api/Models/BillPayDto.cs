using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.admin.Models
{

    public class BillPayDto
    {
        public int BillPayID {get; set;}

        [Display(Name = "Account Number")]
        [ForeignKey("Account")]

        [Required]
        // FK
        public int AccountNumber { get; set; }
        public virtual Account Account {get; set;}

        [Required]
        [Display(Name = "Payee ID")]
        public int PayeeID { get; set; }

        [Column(TypeName = "money")]
        [DataType(DataType.Currency), Required]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Scheduled Tiem UTC")]
        public DateTime ScheduledTimeUtc { get; set; }

        [Required, Column(TypeName = "char")]
        public Char Period {get; set; }
        public Boolean Failed {get; set; }
        [Display(Name = "Payment Locked")]
        public Boolean PaymentLocked {get; set;}
    }
}