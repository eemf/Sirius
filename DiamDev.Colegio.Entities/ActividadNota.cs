using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Actividad_Nota")]
    public class ActividadNota
    {
        [Key, Column(name: "Actividad_Id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ActividadId { get; set; }

        [ForeignKey("ActividadId")]
        public Actividad Actividad { get; set; }

        [Key, Column(name: "Alumno_Id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AlumnoId { get; set; }

        [ForeignKey("AlumnoId")]
        public Alumno Alumno { get; set; }

        public decimal Nota { get; set; }

        [StringLength(300)]
        public string Observacion { get; set; }

        [Column("Responsable_Id")]
        public long ResponsableId { get; set; }

        [ForeignKey("ResponsableId")]
        public Usuario Responsable { get; set; }

        public DateTime Fecha { get; set; }
    }
}
