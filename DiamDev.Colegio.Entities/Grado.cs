using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Grado")]
    public class Grado
    {
        [Key, Column("Grado_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long GradoId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }     

        [Column("Nivel_Id")]
        public long NivelId { get; set; }

        [ForeignKey("NivelId")]
        public NivelAcademico Nivel { get; set; }

        [Column("Jornada_Id")]
        public long JornadaId { get; set; }

        [ForeignKey("JornadaId")]
        public Jornada Jornada { get; set; }

        [Required(ErrorMessage = "El grado es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public decimal Precio { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
