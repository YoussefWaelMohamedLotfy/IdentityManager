using IdentityManager.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace IdentityManager.Services
{
    public class SendGridEmailService : IEmailSender
    {
        private SendGridOptions _options { get; set; }

        public SendGridEmailService(IOptions<SendGridOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string recipientEmail, string subject, string message)
        {
            await Execute(_options.ApiKey, subject, message, recipientEmail);
        }

        private async Task<Response> Execute(string apiKey, string subject, string message, string receipientEmail)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_options.SenderEmail, _options.SenderName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(receipientEmail));

            return await client.SendEmailAsync(msg);
        }
    }
}
