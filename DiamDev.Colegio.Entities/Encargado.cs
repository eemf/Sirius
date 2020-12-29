using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Colegio_Encargado")]
    public class Encargado
    {
        [Key, Column("Encargado_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long EncargadoId { get; set; }

        [Column("Colegio_Id")]
        public long ColegioId { get; set; }

        [Column("Tipo_Id")]
        public long TipoId { get; set; }

        [ForeignKey("TipoId")]
        public TipoEncargado Tipo { get; set; }

        [Column("Genero_Id")]
        public long GeneroId { get; set; }

        [ForeignKey("GeneroId")]
        public Genero Genero { get; set; }

        [Column("Estado_Civil_Id")]
        public long EstadoCivilId { get; set; }

        [ForeignKey("EstadoCivilId")]
        public EstadoCivil EstadoCivil { get; set; }

        [Column("Primer_Nombre")]
        [StringLength(50)]
        public string PrimerNombre { get; set; }

        [Column("Segundo_Nombre")]
        [StringLength(50)]
        public string SegundoNombre { get; set; }

        [Column("Primer_Apellido")]
        [StringLength(50)]
        public string PrimerApellido { get; set; }

        [Column("Segundo_Apellido")]
        [StringLength(50)]
        public string SegundoApellido { get; set; }

        [StringLength(200)]
        public string Nombre { get; set; }

        [Column("Fecha_Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Column("Lugar_Nacimiento")]
        public string LugarNacimiento { get; set; }

        public string Profesion { get; set; }

        [StringLength(500)]
        public string Direccion { get; set; }

        [Column("Correo_Electronico")]
        [StringLength(150)]
        public string CorreoElectronico { get; set; }

        [StringLength(15)]
        [Column("Telefono_Celular")]
        public string TelefonoCelular { get; set; }

        [StringLength(15)]
        [Column("Telefono_Casa")]
        public string TelefonoCasa { get; set; }

        [StringLength(15)]
        [Column("Telefono_Oficina")]
        public string TelefonoOficina { get; set; }

        [Column("Nombre_Empresa")]
        public string NombreEmpresa { get; set; }

        public string Observaciones { get; set; }

        public bool Activo { get; set; }

        public DateTime Fecha { get; set; }

        public int Correlativo { get; set; }

        public List<EncargadoAlumno> Alumnos { get; set; }

        [NotMapped]
        public Usuario Usuario { get; set; }
    }
}
