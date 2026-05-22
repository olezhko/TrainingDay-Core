using Microsoft.Extensions.Options;
using TrainingDay.Web.Data.Repositories;
using TrainingDay.Web.Data.Support;
using TrainingDay.Web.Services.Email;
using TrainingDay.Web.Services.Mapping;

namespace TrainingDay.Web.Services.Support;

public class SupportManager(IEmailSender emailSender, IOptions<EmailSettings> emailSettings, ISupportRepository supportRepository) : ISupportManager
{
    public async Task SendContactMe(ContactMeModel model, CancellationToken cancellationToken)
    {
        var supportRequest = model.ToEntity();
        await supportRepository.Create(supportRequest, cancellationToken);

        await emailSender.SendEmailAsync(emailSettings.Value.SupportEmail, "TrainingDay ContactMe", $"{model.Name} - {model.Email}. Message: {model.Message}");
    }
}
