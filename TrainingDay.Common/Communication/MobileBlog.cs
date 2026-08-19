using System.Text.Json.Serialization;

namespace TrainingDay.Common.Communication;

public sealed record BlogResponse
{
	[JsonPropertyName("id")]
    public int Id { get; set; }

	/// <summary>
	/// http-content needed to decode
	/// </summary>
	[JsonPropertyName("content")]
	public string? Content { get; set; }

	/// <summary>
	/// DateTime
	/// </summary>
	[JsonPropertyName("published")]	
	public DateTime Published { get; set; }
	
	[JsonPropertyName("title")]
	public required string Title { get; set; }

	[JsonPropertyName("tags")]
	public List<string> Tags { get; set; } = [];
}