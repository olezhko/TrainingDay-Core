using TrainingDay.Web.Entities;
using TrainingDay.Web.Services.MobileTokens.Models;

namespace TrainingDay.Web.Services.Mapping;

public static class MobileTokenMappingExtensions
{
    public static MobileTokenModel ToModel(this MobileToken entity) => new MobileTokenModel
    {
        Id = entity.Id,
        Token = entity.Token,
        Language = entity.Language,
        Zone = entity.Zone,
        LastSend = entity.LastSend,
        LastWorkoutDateTime = entity.LastWorkoutDateTime,
        LastBodyControlDateTime = entity.LastBodyControlDateTime,
    };
}
