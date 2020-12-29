﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiamDev.Colegio.Entities
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Menu_Id")]
        public int MenuId { get; set; }

        [Column("Menu_Padre_Id")]
        public int? MenuPadreId { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [Required]
        public int Orden { get; set; }

        [StringLength(50)]
        public string IconName { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Permiso_Id")]
        public string PermisoId { get; set; }

        [ForeignKey("PermisoId")]
        public Permiso Permiso { get; set; }

        [NotMapped]
        public List<Menu> Items { get; set; }
    }
}
