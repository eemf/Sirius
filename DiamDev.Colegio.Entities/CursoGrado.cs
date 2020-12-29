using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Curso_Grado")]
    public class CursoGrado
    {
        [Key, Column(name: "Curso_Id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }

        [Key, Column(name: "Grado_Id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long GradoId { get; set; }

        [ForeignKey("GradoId")]
        public Grado Grado { get; set; }
    }
}
