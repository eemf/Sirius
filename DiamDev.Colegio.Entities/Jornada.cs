using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Jornada")]
    public class Jornada
    {
        [Key, Column("Jornada_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long JornadaId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }      

        [Required(ErrorMessage = "El jornada del colegio es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
