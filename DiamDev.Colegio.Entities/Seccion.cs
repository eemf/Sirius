using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Seccion")]
    public class Seccion
    {
        [Key, Column("Seccion_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SeccionId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }      

        [Required(ErrorMessage = "La seccion del grado es requerida")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
