namespace TrainingDay.Web.Entities;

public class SupportRequest
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
}