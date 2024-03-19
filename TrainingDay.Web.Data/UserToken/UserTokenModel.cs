using TrainingDay.Web.Entities;

namespace TrainingDay.Web.Data.UserToken;

public class UserTokenModel
{
    public List<MobileToken> Tokens { get; set; } = new List<MobileToken>();

    public string UserMail { get; set; }

    public string UserName { get; set; }

    public Guid UserId { get; set; }
}
