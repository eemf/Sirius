using System.Collections.Generic;

namespace DiamDev.Colegio.Entities
{
    public class AlumnoxResponsable
    {
        public long EncargadoId { get; set; }

        public long ResponsableId { get; set; }

        public long GradoId { get; set; }

        public long SeccionId { get; set; }

        public long AlumnoId { get; set; }

        public string Ciclo { get; set; }

        public string Alumno { get; set; }

        public string Grado { get; set; }

        public string Seccion { get; set; }

        public string Jornada { get; set; }

        public List<NotaxCursoModel> Cursos { get; set; }
    }
}
