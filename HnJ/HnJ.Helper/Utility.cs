using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HnJ.Helper
{
    public class Utility
    {
        public static string GetAbsoluteUrl
        {
            get
            {
                HttpContext context = HttpContext.Current;
                string baseUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Authority + context.Request.ApplicationPath.TrimEnd('/') + '/';
                return baseUrl;
            }
        }

        public static void SendEmail(string to, string body)
        {
            var subject = "Your Booking";
            var address = "hnjcinemas@gmail.com";

            MailMessage mail = new MailMessage(new MailAddress(address), new MailAddress(to));

            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            SmtpClient smtpMail = new SmtpClient("smtp.gmail.com");

            smtpMail.Port = 587;
            smtpMail.EnableSsl = true;
            smtpMail.Credentials = new NetworkCredential(address, "nizamani");

            smtpMail.Send(mail);
        }
    }
}
