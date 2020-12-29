using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Estado_Civil")]
    public class EstadoCivil
    {
        [Key, Column("Estado_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long EstadoId { get; set; }
                     
        [Required(ErrorMessage = "El estado civil es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
