using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities.MobileItems;

public class UserExercise : Common.Models.Exercise, IUserEntity
{
    public UserExercise()
    {

    }
    public UserExercise(Common.Models.Exercise item)
    {
        DatabaseId = item.Id;
        Description = item.Description;
        Name = item.Name;
        MusclesString = item.MusclesString;
        TagsValue = item.TagsValue;
        CodeNum = item.CodeNum;
    }
    [Key]
    public new int Id { get; set; }
    public Guid UserId { get; set; }
    public int DatabaseId { get; set; }

    public virtual User User { get; set; }
}