using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Rol_Permiso")]
    public class RolPermiso
    {
        [Key, Column(name: "Rol_Id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long RolId { get; set; }

        [Key, Column(name: "Permiso_Id", Order = 1)]
        [StringLength(100)]
        public string PermisoId { get; set; }

        [ForeignKey("PermisoId")]
        public Permiso Permiso { get; set; }
    }
}
