using System.ComponentModel.DataAnnotations;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    
    public virtual ICollection<UserAlarm> UserAlarms { get; set; }
    public virtual ICollection<UserSuperSet> UserSuperSets { get; set; }
    public virtual ICollection<UserExercise> UserExercises { get; set; }
    public virtual ICollection<UserLastTraining> UserLastTrainings { get; set; }
    public virtual ICollection<UserLastTrainingExercise> UserLastTrainingExercises { get; set; }
    public virtual UserMobileToken UserMobileToken { get; set; }
    public virtual ICollection<UserTraining> UserTrainings { get; set; }
    public virtual ICollection<UserTrainingExercise> UserTrainingExercises { get; set; }
    public virtual ICollection<UserTrainingGroup> UserTrainingGroups { get; set; }
    public virtual ICollection<UserWeightNote> UserWeightNotes { get; set; }
}