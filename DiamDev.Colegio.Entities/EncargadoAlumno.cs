using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Encargado_Alumno")]
    public class EncargadoAlumno
    {
        [Key, Column(name: "Encargado_Id", Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long EncargadoId { get; set; }

        [ForeignKey("EncargadoId")]
        public Encargado Encargado { get; set; }

        [Key, Column(name: "Alumno_Id", Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long AlumnoId { get; set; }

        [ForeignKey("AlumnoId")]
        public Alumno Alumno { get; set; }
    }
}
