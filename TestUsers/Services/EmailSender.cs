using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TestUsers.Models;

namespace TestUsers.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public EmailSettings _emailSettings { get; }

        public Task SendEmailAsync(string email, string subject, string message)
        {

            Execute(email, subject, message).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute(string email, string subject, string message)
        {
            //message qui sera le coprs du mail
            string monContenuMail = "<h1 style =\" color: #000000; padding: 14px 18px 14px 18px; background: linear-gradient(to right, #f39e84 0%, #e89496 50%, #c65a73 100%);\">Pour commencer vos aventure, merci de confirmer votre inscription,</h1>" +
            "<h3 style =\" color: #0d0d0d \">Bien le bonjour jeune héros</h3>" +
            "<h4 style =\" color: #0d0d0d \">Vous venez de vous inscrire sur le site leheroscestvous.JDR, mais avant de commencer vos aventures il faut confirmer votre inscription. </h4>" +
            "<h4 style =\" color: #0d0d0d \">Pas question qu'un fourbe voleur usurpe votre identité et vous vole vos trésors durement gagner</h4>";

            try
            {
                string toEmail = string.IsNullOrEmpty(email)
                                 ? _emailSettings.ToEmail
                                 : email;
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "Vous êtes un héros - By Eldaran83 -")
                };
                mail.To.Add(new MailAddress(toEmail));
                //   mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                mail.Subject = "Héros vous avez un message ! - " + subject;
                // mail.Body = message;
                mail.Body = monContenuMail + message;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //do something here
            }
        }
    }
}

