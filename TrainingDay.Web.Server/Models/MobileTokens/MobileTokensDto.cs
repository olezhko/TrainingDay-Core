namespace TrainingDay.Web.Server.Models.MobileTokens
{
    public class MobileTokensDto
    {
        public IReadOnlyCollection<MobileTokenDto> MobileTokens { get; set; } = [];

        public int TotalCount { get; set; }
    }
}
