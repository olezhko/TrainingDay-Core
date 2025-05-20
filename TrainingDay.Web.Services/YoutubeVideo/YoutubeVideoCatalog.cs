using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;
using System.Text;
using TrainingDay.Common.Communication;

namespace TrainingDay.Web.Services.YoutubeVideo
{
    public class YoutubeVideoCatalog : IYoutubeVideoCatalog
	{
		private readonly ApiSettings _settings;

		public YoutubeVideoCatalog(IOptions<ApiSettings> settings)
		{
			_settings = settings.Value;
		}

		public async Task<List<YoutubeVideoItem>> GetVideoItemsAsync(string search, int maxCount = 10)
		{
			var result = new List<YoutubeVideoItem>();

			Console.OutputEncoding = Encoding.Unicode;
			var youtubeService = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = _settings.YouTube.Key,
				ApplicationName = _settings.YouTube.AppName,
			});

			var searchListRequest = youtubeService.Search.List("snippet");
			searchListRequest.Q = search; // Replace with your search term.
			searchListRequest.MaxResults = maxCount;

			// Call the search.list method to retrieve results matching the specified query term.
			var searchListResponse = await searchListRequest.ExecuteAsync();

			// Add each result to the appropriate list, and then display the lists of
			// matching videos, channels, and playlists.
			foreach (var searchResult in searchListResponse.Items)
			{
				switch (searchResult.Id.Kind)
				{
					case "youtube#video":
						result.Add(new YoutubeVideoItem()
						{
							VideoAuthor = searchResult.Snippet.ChannelTitle,
							VideoTitle = searchResult.Snippet.Title,
							VideoUrl = searchResult.Id.VideoId
						});
						break;
				}
				if (result.Count == maxCount)
				{
					break;
				}
			}

			return result;
		}
	}
}
