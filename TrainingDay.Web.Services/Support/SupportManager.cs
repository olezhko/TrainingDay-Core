using TrainingDay.Web.Data.Support;
using TrainingDay.Web.Services.Email;

namespace TrainingDay.Web.Services.Support;

public class SupportManager(IEmailSender emailSender) : ISupportManager
{
    public async Task SendContactMe(ContactMeModel model)
    {
        // ToDo: Save to DB, replace mail to support email
        await emailSender.SendEmailAsync("alezhko.work@gmail.com", "TrainingDay ContactMe", $"{model.Name} - {model.Email}. Message: {model.Message}");
    }
}