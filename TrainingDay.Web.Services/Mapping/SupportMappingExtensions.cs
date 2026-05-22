using TrainingDay.Web.Data.Support;
using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Services.Mapping;

public static class SupportMappingExtensions
{
    public static SupportRequest ToEntity(this ContactMeModel model) => new SupportRequest
    {
        Name = model.Name,
        Email = model.Email,
        Message = model.Message,
    };
}
