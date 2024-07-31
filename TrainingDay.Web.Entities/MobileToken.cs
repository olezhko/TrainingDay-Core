using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities;

public class MobileToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; }
    public string Language { get; set; }
    public string Zone { get; set; }

    public DateTime LastSend { get; set; } //last enter to application
    public DateTime LastWorkoutDateTime { get; set; } // UTC
    public DateTime LastBodyControlDateTime { get; set; } // UTC
}