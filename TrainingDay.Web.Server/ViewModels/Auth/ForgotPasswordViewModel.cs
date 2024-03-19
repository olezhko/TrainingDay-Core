using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Server.ViewModels.Auth;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}