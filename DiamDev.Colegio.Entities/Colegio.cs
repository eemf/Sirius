using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio")]
    public class Colegio
    {
        [Key, Column("Colegio_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ColegioId { get; set; }     

        [Required(ErrorMessage = "El nombre del colegio es requerido")]
        [StringLength(300)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Direccion { get; set; }

        [StringLength(15)]
        public string Telefono { get; set; }

        [StringLength(300)]
        public string Contacto { get; set; }

        [Column("Telefono_Contacto")]
        [StringLength(15)]
        public string TelefonoContacto { get; set; }

        [StringLength(15)]
        public string Alias { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }

        [NotMapped]
        public string Logo { get; set; }

        [NotMapped]
        public ColegioLogo Fotografia { get; set; }
    }
}
