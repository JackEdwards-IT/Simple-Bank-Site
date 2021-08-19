using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.Models
{
    public record Customer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Must be 4 digits")]
        public int CustomerID { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; }

        [StringLength(11)]
        public string Tfn { get; set; }

        [StringLength(50)]
        public string Address { get; set; }

        [StringLength(40)]
        public string Suburb { get; set; }

        [ StringLength(3), Column(TypeName = "nvarchar")]
        public string State { get; set; }

        [StringLength(4)]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "Must be 4 digits")]
        public string PostCode { get; set; }

        [StringLength(12)]
        [RegularExpression("^04[0-9]{8}$", ErrorMessage = "Must be a valid mobile number")]
        public string Mobile { get; set; }
        public Boolean AccountLocked { get; set; }
        public virtual List<Account> Accounts { get; set; }
    }
}
