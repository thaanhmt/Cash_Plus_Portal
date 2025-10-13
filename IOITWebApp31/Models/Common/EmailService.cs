using log4net;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace IOITWebApp31.Models.Common
{
    public class EmailService
    {
        private static readonly ILog log = LogMaster.GetLogger("email", "email");
        /// smtp của host: ví dụ smtp.gmail.com
        /// port của host: ví dụ gmail là 587

        /// 
        public static bool Send(string smtpUserName, string smtpPassword, string smtpHost, int smtpPort,
            string toEmail, string subject, string body)
        {
            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    smtpClient.EnableSsl = false;
                    smtpClient.Host = smtpHost;
                    smtpClient.Port = smtpPort;
                    smtpClient.UseDefaultCredentials = true;
                    smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                    var msg = new MailMessage
                    {
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        From = new MailAddress(smtpUserName),
                        Subject = subject,
                        Body = body,
                        Priority = MailPriority.Normal,
                    };

                    msg.To.Add(toEmail);

                    smtpClient.Send(msg);
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
    }
}