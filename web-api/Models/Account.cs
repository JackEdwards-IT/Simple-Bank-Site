using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.admin.Models
{
    public static class AccountType 
    {
        public const char Checking = 'C';
        public const char Saving = 'S';
    }

    public class Account
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Account Number")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Must be 4 digits")]
        public int AccountNumber { get; set; }

        [Required, Display(Name = "Type"), Column(TypeName = "char")]
        public char AccountType { get; set; }

        // FK
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
    }
}
