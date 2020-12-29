using DiamDev.Colegio.Entities;
using System.Data.Entity;

namespace DiamDev.Colegio.DAL
{
    public class ColegioContext : DbContext
    {
        public DbSet<Colegio.Entities.Colegio> Colegio { get; set; }

        public DbSet<Permiso> Permiso { get; set; }

        public DbSet<Menu> Menu { get; set; }

        public DbSet<Rol> Rol { get; set; }

        public DbSet<RolPermiso> RolPermiso { get; set; }

        public DbSet<Usuario> Usuario { get; set; }

        public DbSet<Ciclo> Ciclo { get; set; }

        public DbSet<NivelAcademico> NivelAcademico { get; set; }

        public DbSet<Seccion> Seccion { get; set; }

        public DbSet<Jornada> Jornada { get; set; }

        public DbSet<Grado> Grado { get; set; }

        public DbSet<TipoPonderacion> TipoPonderacion { get; set; }

        public DbSet<Curso> Curso { get; set; }

        public DbSet<CursoGrado> CursoGrado { get; set; }

        public DbSet<Maestro> Maestro { get; set; }

        public DbSet<Genero> Genero { get; set; }

        public DbSet<EstadoCivil> EstadoCivil { get; set; }

        public DbSet<Alumno> Alumno { get; set; }

        public DbSet<TipoEncargado> TipoEncargado { get; set; }

        public DbSet<Encargado> Encargado { get; set; }

        public DbSet<EncargadoAlumno> EncargadoAlumno { get; set; }

        public DbSet<Inscripcion> Inscripcion { get; set; }

        public DbSet<MaestroCurso> MaestroCurso { get; set; }

        public DbSet<Unidad> Unidad { get; set; }

        public DbSet<ActividadTipo> ActividadTipo { get; set; }

        public DbSet<Actividad> Actividad { get; set; }        

        public DbSet<Asistencia> Asistencia { get; set; }       

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Curso>().HasRequired(c => c.Ciclo).WithMany().HasForeignKey(c => c.CicloId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Inscripcion>().HasRequired(n => n.NivelAcademico).WithMany().HasForeignKey(n => n.NivelAcademicoId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Inscripcion>().HasRequired(j => j.Jornada).WithMany().HasForeignKey(j => j.JornadaId).WillCascadeOnDelete(false);

            modelBuilder.Entity<EncargadoAlumno>().HasRequired(a => a.Alumno).WithMany().HasForeignKey(a => a.AlumnoId).WillCascadeOnDelete(false);

            modelBuilder.Entity<ActividadNota>().HasRequired(r => r.Responsable).WithMany().HasForeignKey(r => r.ResponsableId).WillCascadeOnDelete(false);
        }
    }
}
