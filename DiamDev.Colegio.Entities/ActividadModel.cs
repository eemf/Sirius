using System;

namespace DiamDev.Colegio.Entities
{
    public class ActividadModel
    {
        public long ActividadId { get; set; }

        public string Ciclo { get; set; }

        public string Unidad { get; set; }

        public string Tipo { get; set; }

        public string Actividad { get; set; }

        public string Descripcion { get; set; }

        public decimal Nota { get; set; }

        public DateTime FechaEntrega { get; set; }
    }
}
