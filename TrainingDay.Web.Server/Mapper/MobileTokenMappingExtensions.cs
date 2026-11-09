using TrainingDay.Web.Server.Models.MobileTokens;
using TrainingDay.Web.Server.Models.UserTokens;
using TrainingDay.Web.Services.MobileTokens.Models;
using TrainingDay.Web.Services.UserTokens.Models;

namespace TrainingDay.Web.Server.Mapper;

public static class MobileTokenMappingExtensions
{
    public static MobileTokenDto ToDto(this MobileTokenModel model) => new MobileTokenDto
    {
        Id = model.Id,
        Token = model.Token,
        Language = model.Language,
        Zone = model.Zone,
        LastSend = model.LastSend,
        LastWorkoutDateTime = model.LastWorkoutDateTime,
        LastBodyControlDateTime = model.LastBodyControlDateTime,
    };

    public static UserTokenDto ToDto(this UserTokenModel model) => new UserTokenDto
    {
        Tokens = [.. model.Tokens.Select(item => item.ToDto())],
        UserMail = model.UserMail,
        UserName = model.UserName,
        UserId = model.UserId,
    };
}
