using System.ComponentModel.DataAnnotations;

namespace DiamDev.Colegio.UI.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}