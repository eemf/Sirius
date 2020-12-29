using System;

namespace DiamDev.Colegio.Entities
{
    public class AsistenciaxCursoModel
    {
        public long AlumnoId { get; set; }

        public string Alumno { get; set; }

        public DateTime FechaAsistencia { get; set; }

        public bool Si { get; set; }

        public bool No { get; set; }

        public bool Tarde { get; set; }

        public string Comentario { get; set; }
    }
}
