using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Inscripcion")]
    public class Inscripcion
    {
        [Key, Column("Inscripcion_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long InscripcionId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }

        [Column("Alumno_Id")]
        public long AlumnoId { get; set; }

        [ForeignKey("AlumnoId")]
        public Alumno Alumno { get; set; }

        [Column("Ciclo_Id")]
        public long CicloId { get; set; }

        [ForeignKey("CicloId")]
        public Ciclo Ciclo { get; set; }

        [Column("Nivel_Academico_Id")]
        public long NivelAcademicoId { get; set; }

        [ForeignKey("NivelAcademicoId")]
        public NivelAcademico NivelAcademico { get; set; }

        [Column("Jornada_Id")]
        public long JornadaId { get; set; }

        [ForeignKey("JornadaId")]
        public Jornada Jornada { get; set; }

        [Column("Grado_Id")]
        public long GradoId { get; set; }

        [Column("GradoId")]
        public Grado Grado { get; set; }

        [Column("Seccion_Id")]
        public long SeccionId { get; set; }

        [ForeignKey("SeccionId")]
        public Seccion Seccion { get; set; }      

        public bool Activo { get; set; }

        [Column("Responsable_Id")]
        public long ResponsableId { get; set; }

        [ForeignKey("ResponsableId")]
        public Usuario Responsable { get; set; }

        [Column("Fecha_Hora_Ultima_Modificacion")]
        public DateTime FechaHoraUltimaModificacion { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
