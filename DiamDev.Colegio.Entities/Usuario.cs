using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key, Column(name: "Usuario_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UsuarioId { get; set; }

        [Column("Relacion_Id")]
        public long? RelacionId { get; set; }

        [Column("Colegio_Id")]
        public long? ColegioId { get; set; }

        [ForeignKey("ColegioId")]
        public Colegio Colegio { get; set; }

        [Column("Rol_Id")]
        public long RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        [StringLength(150)]
        [Column("Login_Original")]
        public string LoginOriginal { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(150)]
        public string Login { get; set; }

        [Required(ErrorMessage = "El password es requerido")]
        [StringLength(150)]
        public string Password { get; set; }

        [NotMapped]
        public string NuevoPassword { get; set; }

        [NotMapped]
        public string ConfirmarPassword { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200)]
        public string Nombre { get; set; }

        public DateTime Fecha { get; set; }

        [Column("Fecha_Ultima_Actividad")]
        public DateTime? FechaUltimaActividad { get; set; }

        [Column("Reiniciar_Password")]
        public bool ReiniciarPassword { get; set; }

        public bool Administrador { get; set; }

        public bool Activo { get; set; }

        public int Correlativo { get; set; }

        [NotMapped]
        public List<RolPermiso> RolesPermiso { get; set; }
    }
}
