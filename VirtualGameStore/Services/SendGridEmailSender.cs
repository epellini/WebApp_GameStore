using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace VirtualGameStore.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        // Constructor with send grid client and logger:
        public SendGridEmailSender(ISendGridClient sendGridClient, ILogger<SendGridEmailSender> logger)
        {
            _sendGridClient = sendGridClient;
            _logger = logger;
        }

        // Send email async method:
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("conestogavirtualgamestore@outlook.com", "Conestoga Virtual Game Store"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(toEmail));

            var response = await _sendGridClient.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Email queued successfully");
            }
            else
            {
                _logger.LogError("Failed to send email");
            }
        }

        // Private read only interface fields for send grid client and logger:
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger _logger;
    }
}
