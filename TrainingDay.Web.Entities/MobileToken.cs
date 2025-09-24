using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Entities;

public class MobileToken : AuditableEntity
{
    [Key]
    public int Id { get; set; }
    public required string Token { get; set; }
    public required string Language { get; set; }
    public required string Zone { get; set; }

    public DateTime LastSend { get; set; } //last enter to application
    public DateTime LastWorkoutDateTime { get; set; } // UTC
    public DateTime LastBodyControlDateTime { get; set; } // UTC
}