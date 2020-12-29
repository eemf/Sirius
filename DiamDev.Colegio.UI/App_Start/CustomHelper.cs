using DiamDev.Colegio.BLL;
using DiamDev.Colegio.Entities;
using System.Web;

namespace DiamDev.Colegio.UI.App_Start
{
    public static class CustomHelper
    {
        public static void setUsuario(string Usuario)
        {
            if (HttpContext.Current.Session["Nombre"] == null)
            {
                Usuario UsuarioActual = new UsuarioBL().ObtenerxLogin(Usuario);

                if (UsuarioActual != null)
                {
                    HttpContext.Current.Session["Usuario"] = UsuarioActual;
                    HttpContext.Current.Session["Nombre"] = UsuarioActual.Nombre;
                    HttpContext.Current.Session["ID"] = UsuarioActual.RelacionId == null ? 0 : UsuarioActual.RelacionId.Value;
                }
            }
        }

        public static long getID()
        {
            long Id = 0;

            if (HttpContext.Current.Session["Usuario"] == null)
            {
                Usuario Usuario = new UsuarioBL().ObtenerxLogin(HttpContext.Current.User.Identity.Name);

                if (Usuario != null)
                {
                    HttpContext.Current.Session["ID"] = Usuario.RelacionId == null ? 0 : Usuario.RelacionId.Value;
                    Id = Usuario.RelacionId == null ? 0 : Usuario.RelacionId.Value;
                }
            }
            else
            {
                Usuario Usuario = (Usuario)HttpContext.Current.Session["Usuario"];

                if (Usuario != null)
                {
                    Id = Usuario.RelacionId == null ? 0 : Usuario.RelacionId.Value;
                }
            }

            return Id;
        }

        public static long getUsuarioId()
        {
            long UsuarioId = 0;

            if (HttpContext.Current.Session["Usuario"] == null)
            {
                Usuario Usuario = new UsuarioBL().ObtenerxLogin(HttpContext.Current.User.Identity.Name);

                if (Usuario != null)
                {
                    HttpContext.Current.Session["Usuario"] = Usuario;
                    HttpContext.Current.Session["Nombre"] = Usuario.Nombre;
                    UsuarioId = Usuario.UsuarioId;
                }
            }
            else
            {
                Usuario Usuario = (Usuario)HttpContext.Current.Session["Usuario"];

                if (Usuario != null)
                {
                    UsuarioId = Usuario.UsuarioId;
                }
            }

            return UsuarioId;
        }

        public static string getUsuarioNombre()
        {
            if (HttpContext.Current.Session["Nombre"] == null)
            {
                Usuario Usuario = new UsuarioBL().ObtenerxLogin(HttpContext.Current.User.Identity.Name);

                if (Usuario != null)
                {
                    return Usuario.Nombre;
                }
            }
            else
            {
                Usuario Usuario = (Usuario)HttpContext.Current.Session["Usuario"];

                if (Usuario != null)
                {
                    return Usuario.Nombre;
                }
            }

            return "Usuario No Valido";
        }       

        public static void setTitulo(string Encabezado, string SubEncabezado)
        {
            HttpContext.Current.Session["Encabezado"] = Encabezado;
            HttpContext.Current.Session["SubEncabezado"] = SubEncabezado;
        }

        public static bool Permiso(string Permiso)
        {
            return new RolBL().AutorizacionPermisoxUsuario(HttpContext.Current.User.Identity.Name, Permiso);
        }

        public static void setColegio(Entities.Colegio Colegio)
        {
            if (Colegio != null)
            {
                HttpContext.Current.Session["Colegio"] = Colegio;
            }            
        }

        public static long getColegioId()
        {
            long ColegioId = 0;

            if (HttpContext.Current.Session["Colegio"] != null)
            {
                Entities.Colegio Colegio = (Entities.Colegio)HttpContext.Current.Session["Colegio"];

                if (Colegio != null)
                {
                    ColegioId = Colegio.ColegioId;
                }
            }

            return ColegioId;
        }

        public static string getColegioNombre()
        {
            string Nombre = string.Empty;

            if (HttpContext.Current.Session["Colegio"] != null)
            {
                Entities.Colegio Colegio = (Entities.Colegio)HttpContext.Current.Session["Colegio"];

                if (Colegio != null)
                {
                    Nombre = Colegio.Nombre;
                }
            }
            else
            {
                Nombre = "SIRIUS";
            }

            return Nombre;
        }

        public static Entities.Colegio getColegio()
        {
            Entities.Colegio ColegioActual = new Entities.Colegio();

            if (HttpContext.Current.Session["Colegio"] != null)
            {
                Entities.Colegio Colegio = (Entities.Colegio)HttpContext.Current.Session["Colegio"];

                if (Colegio != null)
                {
                    ColegioActual = Colegio;
                }
            }

            return ColegioActual;
        }       
    }
}