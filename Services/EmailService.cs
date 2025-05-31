using System.Net;
using System.Net.Mail;

namespace Blog.Services
{
    public class EmailService
    {
        public bool Send(string toName, string toEmail, string subject, string body, string fromName = "Equipe xxx", string fromEmail = "seuemailaqui")
        {
            var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);

            smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.Username, Configuration.Smtp.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            var mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, fromName);
            mail.To.Add(new MailAddress(toEmail, toName));
            mail.Body = body;
            mail.Subject = subject;
            mail.IsBodyHtml = true;

            try { 
                smtpClient.Send(mail);
                return true;
            }
            catch(Exception ex) 
            {
                return false;
            }
        }
    }
}
