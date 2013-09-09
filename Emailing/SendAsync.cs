using System;
using System.Net.Mail;

namespace Emailing
{
    public class SendAsync
    {
        public static void SendEmail(string smtpServer, string username, string password, int port, bool ssl, MailMessage m, bool Async)
        {
            SmtpClient smtpClient = new SmtpClient(smtpServer);
            System.Net.NetworkCredential theCredential = new System.Net.NetworkCredential(username, password);
            smtpClient.Port = port;
            smtpClient.EnableSsl = ssl;
            smtpClient.Credentials = theCredential;
            if (Async)
            {
                SendEmailDelegate sd = (smtpClient.Send);
                AsyncCallback cb = (SendEmailResponse);
                sd.BeginInvoke(m, cb, sd);
            }
            else
            {
                smtpClient.Send(m);
            }

        }
        private delegate void SendEmailDelegate(MailMessage m);
        private static void SendEmailResponse(IAsyncResult ar)
        {
            SendEmailDelegate sd = (SendEmailDelegate)(ar.AsyncState);

            sd.EndInvoke(ar);
        }
    }
}
