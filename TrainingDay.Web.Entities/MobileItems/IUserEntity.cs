namespace TrainingDay.Web.Entities.MobileItems;

public interface IUserEntity
{
    Guid UserId { get; set; }
    int DatabaseId { get; set; }
}