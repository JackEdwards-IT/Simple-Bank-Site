using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.Models
{
    public static class State 
    {
        public const string NSW = "NSW";
        public const string VIC = "VIC";
        public const string QLD = "QLD";
        public const string SA = "SA";
        public const string TAS = "TAS";
        public const string WA = "WA";
        public const string NT = "NT";
    }
    public class Payee
    {
        public int PayeeID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Address { get; set; }

        [Required, StringLength(40)]
        public string Suburb { get; set; }

        [Required, StringLength(3), Column(TypeName = "nvarchar")]
        public string State { get; set; }

        [Required, RegularExpression("^[0-9]{4}$", ErrorMessage = "Must be 4 digits")]
        public string PostCode { get; set; }

        [Required, StringLength(12)]
        [RegularExpression("^04[0-9]{8}$", ErrorMessage = "Must be a valid mobile number")]
        public string Phone { get; set; }

         // FK
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
    }
}