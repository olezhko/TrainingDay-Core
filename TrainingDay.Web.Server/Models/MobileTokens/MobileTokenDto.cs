namespace TrainingDay.Web.Server.Models.MobileTokens
{
    public class MobileTokenDto
    {
        public int Id { get; set; }
        public required string Token { get; set; }
        public required string Language { get; set; }

        /// <summary>
        /// Time zone, e.g. "03:00"
        /// </summary>
        public required string Zone { get; set; }

        /// <summary>
        /// Last enter to application
        /// </summary>
        public DateTime LastSend { get; set; }

        /// <summary>
        /// UTC, last workout date
        /// </summary>
        public DateTime LastWorkoutDateTime { get; set; }

        /// <summary>
        /// UTC, last body control date
        /// </summary>
        public DateTime LastBodyControlDateTime { get; set; }
    }
}
