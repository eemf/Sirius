using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Asistencia")]
    public class Asistencia
    {
        [Key, Column("Asistencia_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ActividadId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }

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

        [Column(name: "Ciclo_Id")]
        public long CicloId { get; set; }

        [ForeignKey("CicloId")]
        public Ciclo Ciclo { get; set; }

        [Column("Fecha_Asistencia")]
        public DateTime FechaAsistencia { get; set; }

        [Column(name: "Alumno_Id")]        
        public long AlumnoId { get; set; }

        [ForeignKey("AlumnoId")]
        public Alumno Alumno { get; set; }

        public bool Si { get; set; }

        public bool No { get; set; }

        public bool Tarde { get; set; }

        public string Comentario { get; set; }

        [Column("Responsable_Id")]
        public long ResponsableId { get; set; }

        [ForeignKey("ResponsableId")]
        public Usuario Responsable { get; set; }

        public DateTime Fecha { get; set; }      
    }
}
