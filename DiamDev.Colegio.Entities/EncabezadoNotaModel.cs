namespace DiamDev.Colegio.Entities
{
    public class EncabezadoNotaModel
    {
        public long InscripcionId { get; set; }

        public long AlumnoId { get; set; }

        public string Alumno { get; set; }

        public string Ciclo { get; set; }

        public string Grado { get; set; }

        public string Seccion { get; set; }

        public string Jornada { get; set; }
    }
}
