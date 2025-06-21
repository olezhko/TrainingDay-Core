namespace TrainingDay.Web.Data.OpenAI
{
    public class OpenAISettings
    {
        public string Key { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-3.5-turbo"; // Default model, can be overridden
        public int MaxTokens { get; set; } = 1000; // Default max tokens, can be overridden
        public float Temperature { get; set; } = 0.7f; // Default temperature, can be overridden
    }
}
