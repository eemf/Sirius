using System;
using System.Collections.Generic;

namespace DiamDev.Colegio.Entities
{
    public class CursoModel
    {
        public long CursoId { get; set; }

        public long ColegioId { get; set; }

        public long GradoId { get; set; }

        public long SeccionId { get; set; }

        public long AlumnoId { get; set; }

        public string Ciclo { get; set; }

        public string Curso { get; set; }

        public string Grado { get; set; }

        public string Seccion { get; set; }

        public DateTime Fecha { get; set; }

        public List<CuadroxCursoModel> Cuadros { get; set; }

        public List<AsistenciaxCursoModel> Asistencias { get; set; }
    }
}
