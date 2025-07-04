namespace TrainingDay.Common.Communication;

public class BlogsResponse
{
	public IReadOnlyCollection<BlogResponse> Items { get; set; }
	public int Page { get; set; }
}

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
	public string Published { get; set; }
	public string Title { get; set; }
	public IReadOnlyCollection<string> Labels { get; set; }
}