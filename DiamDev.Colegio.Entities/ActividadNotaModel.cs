namespace DiamDev.Colegio.Entities
{
    public class ActividadNotaModel
    {
        public long ActividadId { get; set; }

        public long AlumnoId { get; set; }

        public decimal Nota { get; set; }

        public string Comentario { get; set; }
    }
}
