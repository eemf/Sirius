using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Curso")]
    public class Curso
    {
        [Key, Column("Curso_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CursoId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }

        [Column(name: "Ciclo_Id")]
        public long CicloId { get; set; }

        [ForeignKey("CicloId")]
        public Ciclo Ciclo { get; set; }

        [Column("Tipo_Id")]
        public long TipoId { get; set; }

        [ForeignKey("TipoId")]
        public TipoPonderacion Tipo { get; set; }

        [Required(ErrorMessage = "El curso del colegio es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Abreviatura { get; set; }

        public bool Ministerial { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }

        public List<CursoGrado> Grados { get; set; }

        [NotMapped]
        public int Nota { get; set; }

        [NotMapped]
        public long ResponsableId { get; set; }
    }
}
