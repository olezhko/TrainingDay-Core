using TrainingDay.Web.Data.Support;

namespace TrainingDay.Web.Services.Support;

public interface ISupportManager
{
    Task SendContactMe(ContactMeModel model);
}