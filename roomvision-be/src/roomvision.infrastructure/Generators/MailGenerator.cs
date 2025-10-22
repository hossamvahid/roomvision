using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using roomvision.domain.Interfaces.Generators;

namespace roomvision.infrastructure.Generators
{
    public class MailGenerator : IMailGenerator
    {

        private string _smtpServer { get; }
        private int _smtpPort { get; }
        private string _smtpUser { get; }
        private string _smtpPassword { get; }

        public MailGenerator()
        {
            _smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER")!.ToString();
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")!);
            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER")!.ToString();
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASS")!.ToString();
        }

        public async Task WelcomeEmailAsync(string toAddress, string password)
        {
            string subject = "Welcome to RoomVision!";
            string body = "Welcome to RoomVision! We are glad you joined our staff. " +
                "Below you will find your account information.\n\n" +
                $"Email: {toAddress}\nPassword: {password}\n\n" +
                "For security reasons, we recommend that you log in to your account as soon as possible and change your password to one that only you know. You can change your password by accessing the account settings after logging in.";
            
            using(SmtpClient client = new SmtpClient(_smtpServer, _smtpPort) {EnableSsl = true})
            {
                client.Credentials = new System.Net.NetworkCredential(_smtpUser, _smtpPassword);

                MailAddress fromAddress = new MailAddress(_smtpUser, "RoomVision Team");

                MailMessage mailMessage = new MailMessage()
                {
                    From = fromAddress,
                    Subject = subject,
                    Body = body,
                    BodyEncoding = System.Text.Encoding.UTF8
                };

                mailMessage.To.Add(toAddress);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}