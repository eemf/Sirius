using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Genero")]
    public class Genero
    {
        [Key, Column("Genero_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long GeneroId { get; set; }
             
        [Required(ErrorMessage = "El genero es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }
    }
}
