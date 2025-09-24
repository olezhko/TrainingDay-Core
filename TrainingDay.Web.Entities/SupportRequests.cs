namespace TrainingDay.Web.Entities;

public sealed class SupportRequest
{
    public Guid Id { get; set; }
    public required string Message { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime Created { get; set; }
}