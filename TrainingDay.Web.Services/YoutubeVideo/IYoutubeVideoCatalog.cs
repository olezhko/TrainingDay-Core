using TrainingDay.Common;

namespace TrainingDay.Web.Services.YoutubeVideo
{
	public interface IYoutubeVideoCatalog
	{
		Task<List<YoutubeVideoItem>> GetVideoItemsAsync(string search, int maxCount = 10);
	}
}
