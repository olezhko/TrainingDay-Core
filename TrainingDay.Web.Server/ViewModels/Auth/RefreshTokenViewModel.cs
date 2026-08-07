using System.ComponentModel.DataAnnotations;

namespace TrainingDay.Web.Server.ViewModels.Auth;

public class RefreshTokenViewModel
{
    [Required]
    public string RefreshToken { get; set; }
}
