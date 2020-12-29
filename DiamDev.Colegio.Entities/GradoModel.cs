using System.Collections.Generic;

namespace DiamDev.Colegio.Entities
{
    public class GradoModel
    {
        public long GradoId { get; set; }

        public long SeccionId { get; set; }

        public string Ciclo { get; set; }

        public string Grado { get; set; }

        public string Seccion { get; set; }

        public List<AlumnoxGrado> Alumnos { get; set; }
    }
}
