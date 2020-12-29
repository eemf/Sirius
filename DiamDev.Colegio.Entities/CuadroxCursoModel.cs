using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamDev.Colegio.Entities
{
    public class CuadroxCursoModel
    {
        public long ActividadId { get; set; }

        public long UnidadId { get; set; }

        public long AlumnoId { get; set; }

        public string Unidad { get; set; }

        public string Actividad { get; set; }

        public decimal NotaActividad { get; set; }

        public DateTime FechaEntrega { get; set; }

        public string Alumno { get; set; }

        public decimal Nota { get; set; }

        public string Comentario { get; set; }
    }
}
