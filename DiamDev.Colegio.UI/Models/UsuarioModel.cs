using System.ComponentModel.DataAnnotations;

namespace DiamDev.Colegio.UI.Models
{
    public class UsuarioModel
    {
        public long UsuarioId { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50)]
        public string Login { get; set; }

        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [StringLength(150)]
        public string PasswordActual { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(150)]
        public string PasswordNuevo { get; set; }

        [Required(ErrorMessage = "La confirmacion de la contraseña es requerida")]
        [StringLength(150)]
        public string PasswordConfirmacion { get; set; }
    }
}