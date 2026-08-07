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

    public static Task SendNewPasswordAsync(this IEmailSender emailSender, string email, string newPassword)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<p>Dear User,</p>");
        sb.Append("<p></p>");
        sb.Append("<p>We received a request to reset your TrainingDay password. Your new password is:</p>");
        sb.Append($"<p><strong>{HtmlEncoder.Default.Encode(newPassword)}</strong></p>");
        sb.Append("<p>Please sign in with this password and change it as soon as possible.</p>");
        sb.Append("<p></p>");
        sb.Append("<p>If you did not request a password reset, please contact us immediately.</p>");
        sb.Append("<p></p>");
        sb.Append("<p>Best regards,</p>");
        sb.Append("<p>TrainingDay Team</p>");

        return emailSender.SendEmailAsync(email, "Your new TrainingDay password", sb.ToString());
    }

    public static Task SendNewPasswordWithConfirmationAsync(this IEmailSender emailSender, string email, string newPassword, string confirmationLink)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<p>Dear User,</p>");
        sb.Append("<p></p>");
        sb.Append("<p>We received a request to reset your TrainingDay password. Your email address has not been confirmed yet, so please confirm it first: ");
        sb.Append($"<a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>confirm your email</a></p>");
        sb.Append("<p>Once confirmed, you can sign in using your new password:</p>");
        sb.Append($"<p><strong>{HtmlEncoder.Default.Encode(newPassword)}</strong></p>");
        sb.Append("<p>Please change this password as soon as possible after signing in.</p>");
        sb.Append("<p></p>");
        sb.Append("<p>If you did not request a password reset, please contact us immediately.</p>");
        sb.Append("<p></p>");
        sb.Append("<p>Best regards,</p>");
        sb.Append("<p>TrainingDay Team</p>");

        return emailSender.SendEmailAsync(email, "Your new TrainingDay password", sb.ToString());
    }
}