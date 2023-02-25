using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using ZipSwitcher.Config;

namespace ZipSwitcher.Core.Dialout
{
    class EmailDialoutSender : IDialoutSender
    {
        public EmailDialoutSender(string siteName, ConfigDialout config)
        {
            //Set
            this.siteName = siteName;

            //Create client
            client = new SmtpClient(config.SmtpServer, config.SmtpPort);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(config.Username, config.Password);

            //Create from address
            from = new MailAddress(config.FromAddress);

            //Create to addresses
            to = new MailAddress[config.Targets.Length];
            for (int i = 0; i < to.Length; i++)
                to[i] = new MailAddress(config.Targets[i]);
        }

        private readonly string siteName;
        private readonly SmtpClient client;
        private readonly MailAddress from;
        private readonly MailAddress[] to;

        public async Task NotifyAsync(string subject, string body)
        {
            foreach (MailAddress target in to)
            {
                //Create message
                MailMessage msg = new MailMessage(from, target);
                msg.Subject = $"{siteName} Zip Switcher: {subject}";
                msg.SubjectEncoding = Encoding.UTF8;
                msg.IsBodyHtml = false;
                msg.BodyEncoding = Encoding.UTF8;
                msg.Body = body;

                //Send
                try
                {
                    await client.SendMailAsync(msg);
                } catch (Exception ex)
                {
                    ConsoleLogger.Log("DIALOUT", $"Failed to deliver message to {target.Address}: {ex.Message}");
                }
            }
        }
    }
}
