using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Ciclo")]
    public class Ciclo
    {
        [Key, Column("Ciclo_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long CicloId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }    

        [Required(ErrorMessage = "El ciclo escolar del colegio es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
