using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Server.ViewModels.Auth;

public class ResetPasswordViewModel
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public string Code { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
