using System;
using System.Collections.Generic;

namespace DiamDev.Colegio.Entities
{
    public class CursoMaestroModel
    {
        public Guid CursoMaestroId { get; set; }

        public long ColegioId { get; set; }

        public long CursoId { get; set; }

        public long GradoId { get; set; }

        public long SeccionId { get; set; }

        public string Ciclo { get; set; }

        public string Curso { get; set; }

        public string Grado { get; set; }

        public string Seccion { get; set; }

        public int Alumno { get; set; }

        public int Actividad { get; set; }

        public List<ActividadModel> Actividades { get; set; }

        public List<CuadroxCursoModel> Cuadros { get; set; }

        public List<AsistenciaxCursoModel> Asistencias { get; set; }
    }
}
