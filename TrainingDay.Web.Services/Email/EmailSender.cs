using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Text;

namespace TrainingDay.Web.Services.Email
{
    public class EmailSender : IEmailSender
	{
		public EmailSettings _emailSettings { get; }
		public EmailSender(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public Task SendEmailAsync(string email, string subject, string message)
        {
            return DoSendAsync(subject, message, email);
		}

        public async Task<bool> DoSendAsync(string subject, string message, string toEmailAddress, bool isTextBody = false)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailSettings.UsernameEmail);
            email.Subject = subject;

            email.From.Add(new MailboxAddress("TrainingDay", _emailSettings.UsernameEmail));
            email.To.Add(MailboxAddress.Parse(toEmailAddress));

            StringBuilder bodysb = new StringBuilder();
            bodysb.Append("<p style='margin-top:0; padding-top:0;'>");
            bodysb.Append(message);
            bodysb.Append("</p>");

            var builder = new BodyBuilder();
            if (isTextBody)
            {
                builder.TextBody = bodysb.ToString();
            }
            else
            {
                builder.HtmlBody = bodysb.ToString();
            }

            email.Body = builder.ToMessageBody();

            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort, SecureSocketOptions.Auto);
                await smtp.AuthenticateAsync(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
