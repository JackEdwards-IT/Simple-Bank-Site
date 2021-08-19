using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.Models
{
    public class Login
    {
        [Column(TypeName = "nchar")]
        [StringLength(8), Required]
        [RegularExpression("^[0-9]{8}$", ErrorMessage = "Must be 8 digits")]
        
        public string LoginID { get; set; }

        // FK
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Column(TypeName = "nchar")]
        [Required, StringLength(64)]
        public string PasswordHash { get; set; }
    }
}
