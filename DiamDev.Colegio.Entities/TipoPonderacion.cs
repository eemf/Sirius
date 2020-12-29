using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Tipo_Ponderacion")]
    public class TipoPonderacion
    {
        [Key, Column("Tipo_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long TipoId { get; set; }
     
        [Required(ErrorMessage = "El tipo de ponderacion del colegio es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
