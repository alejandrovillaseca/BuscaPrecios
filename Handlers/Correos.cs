using System;
using System.Collections.Generic;
using System.Text;

namespace Handlers
{
    public class Correos
    {
        public static void EnviarCorreo(string titulo, string mensaje)
        {
            //MailMessage msg = new MailMessage();

            //msg.To.Add(ConfigurationManager.AppSettings["EmailPara"]);
            //msg.From = new MailAddress(ConfigurationManager.AppSettings["EmailDe"]);
            //msg.Subject = titulo;
            //msg.Body = mensaje;
            //SmtpClient clienteSmtp = new SmtpClient(ConfigurationManager.AppSettings["ServidorEmail"]);

            //clienteSmtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailNotificaProceso"], ConfigurationManager.AppSettings["EmailNotificaProceso_Password"]);
            //try
            //{
            //    clienteSmtp.Send(msg);
            //}
            //catch (Exception ex)
            //{
            //    Console.Write(ex.Message);
            //    Console.ReadLine();
            //}
        }
    }
}
