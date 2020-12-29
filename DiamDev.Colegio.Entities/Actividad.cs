using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Actividad")]
    public class Actividad
    {
        [Key, Column("Actividad_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ActividadId { get; set; }

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

        [Column(name: "Unidad_Id")]
        public long UnidadId { get; set; }

        [ForeignKey("UnidadId")]
        public Unidad Unidad { get; set; }

        [Column(name: "Tipo_Id")]
        public long TipoId { get; set; }

        [ForeignKey("TipoId")]
        public ActividadTipo Tipo { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Column("Nota_Maxima")]
        public decimal NotaMaxima { get; set; }

        [Column("Fecha_Entrega")]
        public DateTime FechaEntrega { get; set; }

        [Column("Responsable_Id")]
        public long ResponsableId { get; set; }

        [ForeignKey("ResponsableId")]
        public Usuario Responsable { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }

        public List<ActividadNota> Notas { get; set; }
    }
}
