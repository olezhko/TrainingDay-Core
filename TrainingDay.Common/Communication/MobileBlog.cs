namespace TrainingDay.Common.Communication;

public class BlogResponse
{
	public string Guid { get; set; }
	/// <summary>
	/// http-content needed to decode
	/// </summary>
	public string Content { get; set; }
	/// <summary>
	/// DateTime
	/// </summary>
	public DateTime Published { get; set; }
	public string Title { get; set; }
	public IReadOnlyCollection<string> Labels { get; set; }
}