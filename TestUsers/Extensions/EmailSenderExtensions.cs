using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TestUsers.Services;

namespace TestUsers.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "- Confirmer votre inscription ",
                 $"<h2>Cliquez sur le lien et à vous de jouer :<a style='text-decoration: none' href='{HtmlEncoder.Default.Encode(link)}'> ça se passe par ici</a></h2>");

        }
    }
}
