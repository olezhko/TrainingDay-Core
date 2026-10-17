namespace TrainingDay.Web.Data.OpenAI;

public class DeepSeekOptions
{
    public string Key { get; set; } = string.Empty;
    public string Model { get; set; } = "deepseek-chat";
    public string Domain { get; set; } = "api.deepseek.com";
    public float Temperature { get; set; } = 0.7f;
}
