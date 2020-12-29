using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Actividad_Tipo")]
    public class ActividadTipo
    {
        [Key, Column("Tipo_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TipoId { get; set; }
                             
        [Required(ErrorMessage = "El tipo de actividad es requerida")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
