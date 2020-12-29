using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Permiso")]
    public class Permiso
    {
        [Key]
        [StringLength(100)]
        [Column("Permiso_Id")]
        public string PermisoId { get; set; }

        [Required]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(200)]
        public string Modulo { get; set; }
    }
}
