namespace TrainingDay.Web.Entities
{
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
