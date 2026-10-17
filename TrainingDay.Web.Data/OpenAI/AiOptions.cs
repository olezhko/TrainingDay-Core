namespace TrainingDay.Web.Data.OpenAI;

public enum AiProvider
{
    OpenAI,
    DeepSeek
}

public class AiOptions
{
    public AiProvider Provider { get; set; } = AiProvider.OpenAI;
}
