using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Unidad")]
    public class Unidad
    {
        [Key, Column("Unidad_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long UnidadId { get; set; }
                     
        [Required(ErrorMessage = "La unidad es requerida")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
