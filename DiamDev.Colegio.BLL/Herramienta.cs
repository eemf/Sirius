using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiamDev.Colegio.BLL
{
    public class Herramienta
    {
        private string Formato_Inicial(long Id)
        {
            string strCorrelativo = string.Empty;
            string strDigitoRelleno = "0";
            int Cantidad = 3;

            try
            {
                int CantidadNecesaria = Cantidad - Id.ToString().Length;
                strCorrelativo = string.Format("{0}{1}", strDigitoRelleno.PadLeft(CantidadNecesaria, '0'), Id);
            }
            catch (Exception)
            {
                strCorrelativo = string.Empty;
            }

            return strCorrelativo;
        }

        public long Formato_Correlativo(long Id)
        {
            long lngId = 0;
            string strId = string.Empty;
            string strFormato_Inicial = Formato_Inicial(Id);

            try
            {
                if (!string.IsNullOrWhiteSpace(strFormato_Inicial))
                {
                    strId = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMdd"), strFormato_Inicial);

                    if (!long.TryParse(strId, out lngId))
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            { }

            return lngId;
        }

        public bool ValidarEmail(string Email)
        {
            try
            {
                Regex Val = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");

                if (Val.IsMatch(Email))
                {
                    return true;
                }
            }
            catch (Exception)
            { }

            return false;
        }

        public bool ValidarNIT(string Nit)
        {
            try
            {

                if (Nit.Equals("C/F") || Nit.Equals("c/f") || Nit.Equals("CF") || Nit.Equals("cf"))
                {
                    return true;
                }

                int pos = Nit.IndexOf("-");
                string Correlativo = Nit.Substring(0, pos);
                string DigitoVerificador = Nit.Substring(pos + 1);
                int Factor = Correlativo.Length + 1;
                int Suma = 0;
                int Valor = 0;

                for (int x = 0; x <= Nit.IndexOf("-") - 1; x++)
                {
                    Valor = Convert.ToInt32(Nit.Substring(x, 1));
                    Suma = Suma + (Valor * Factor);
                    Factor = Factor - 1;
                }

                double xMOd11 = 0;
                xMOd11 = (11 - (Suma % 11)) % 11;
                string s = Convert.ToString(xMOd11);
                if ((xMOd11 == 10 & DigitoVerificador == "K") | (s.Trim() == DigitoVerificador))
                {
                    return true;
                }
            }
            catch (Exception)
            { }

            return false;
        }

        public static void EnviarCorreo(string Mensaje, string Correo)
        {
            try
            {
                using (MailMessage Mail = new MailMessage())
                {
                    Mail.From = new MailAddress(ConfigurationManager.AppSettings["Correo_Notificacion"].ToString());
                    Mail.To.Add(Correo);
                    Mail.Subject = ConfigurationManager.AppSettings["Titulo_Notificacion"].ToString();

                    Mail.BodyEncoding = System.Text.Encoding.UTF8;
                    Mail.IsBodyHtml = true;
                    Mail.Priority = MailPriority.High;
                    Mail.SubjectEncoding = System.Text.Encoding.UTF8;
                    Mail.Body = Mensaje;

                    using (SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["Smtp_Notificacion"].ToString()))
                    {
                        SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["Puerto_Smtp_Notificacion"].ToString());
                        SmtpServer.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Correo_Notificacion"].ToString(), ConfigurationManager.AppSettings["Password_Notificacion"].ToString());
                        SmtpServer.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL_Notificacion"].ToString());

                        SmtpServer.Send(Mail);
                    }
                }
            }
            catch (Exception)
            { }
        }

        public static void EnviarCorreoAsync(string Mensaje, string Correo)
        {
            Task.Run(() => EnviarCorreo(Mensaje, Correo));
        }
    }
}
