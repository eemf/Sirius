using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Maestro_Curso")]
    public class MaestroCurso
    {
        [Key,Column("Maestro_Curso_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MaestroCursoId { get; set; }
              
        [Column(name: "Maestro_Id")]
        public long MaestroId { get; set; }

        [ForeignKey("MaestroId")]
        public Maestro Maestro { get; set; }

        [Column(name: "Curso_Id")]
        public long CursoId { get; set; }

        [ForeignKey("CursoId")]
        public Curso Curso { get; set; }

        [Column(name: "Grado_Id")]
        public long GradoId { get; set; }

        [ForeignKey("GradoId")]
        public Grado Grado { get; set; }

        [Column(name: "Seccion_Id")]
        public long SeccionId { get; set; }

        [ForeignKey("SeccionId")]
        public Seccion Seccion { get; set; }
    }
}
