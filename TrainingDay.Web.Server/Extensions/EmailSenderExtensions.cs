using System.Text.Encodings.Web;
using System.Text;
using TrainingDay.Web.Services.Email;

namespace TrainingDay.Web.Server.Extensions;

public static class EmailSenderExtensions
{
    public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<p>Dear User,</p>");
        sb.Append("<p></p>");
        sb.Append("<p>Thank you for signing up for TrainingDay. To complete the registration process and start using the app, we need to verify your email address.</p>");
        sb.Append($"<p>Please click on the following link to confirm your email address and activate your account: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a></p>");
        sb.Append("<p>Thank you for choosing TrainingDay. We look forward to helping you achieve your fitness goals.</p>");
        sb.Append("<p></p>");
        sb.Append("<p>Best regards,</p>");
        sb.Append("<p>TrainingDay Team</p>");

        return emailSender.SendEmailAsync(email, "Please confirm your email address for TrainingDay", sb.ToString());
    }
}