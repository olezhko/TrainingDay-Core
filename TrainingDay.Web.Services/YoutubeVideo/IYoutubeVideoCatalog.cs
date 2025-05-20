using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Services.YoutubeVideo
{
	public interface IYoutubeVideoCatalog
	{
		Task<List<YoutubeVideoItem>> GetVideoItemsAsync(string search, int maxCount = 10);
	}
}
