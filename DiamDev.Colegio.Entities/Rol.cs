using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Rol")]
    public class Rol
    {
        [Key, Column(name: "Rol_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RolId { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }

        public List<RolPermiso> Permisos { get; set; }

        [NotMapped]
        public List<Permiso> PermisoIds { get; set; }
    }
}
