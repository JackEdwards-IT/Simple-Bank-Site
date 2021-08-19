using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace a2.admin.Models
{
    public class Login
    {
        [Display(Name = "Name")]
        public string LoginName { get; set; }
        public string Password { get; set; }
    }
}
