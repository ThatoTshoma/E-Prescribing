using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using E_Prescribing.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace E_Prescribing.Services
{
    public class EmailSender //IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com", 
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mycode1997@gmail.com", "mztjzilipukflvcy")
            };

            return client.SendMailAsync("mycode1997@gmail.com", email, subject, htmlMessage);
        }
    }
}
