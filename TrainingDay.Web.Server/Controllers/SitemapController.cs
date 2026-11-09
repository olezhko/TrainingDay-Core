using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using TrainingDay.Web.Data.BlogPosts;
using TrainingDay.Web.Services.Blogs;
using TrainingDay.Web.Services.Exercises;

namespace TrainingDay.Web.Server.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class SitemapController(IBlogPostsManager blogPostsManager, IExerciseManager exerciseManager) : ControllerBase
{
    private const string SiteUrl = "https://trainingday.space";
    private const string WebsiteCulture = "en";

    private static readonly string[] StaticPages = ["/", "/exercises", "/blogs", "/about", "/download", "/contact", "/privacy"];

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> GetAsync(CancellationToken token)
    {
        var blogs = await blogPostsManager.GetSitemapItemsAsync(BlogsCultureTypes.English, token);
        var exerciseCodes = await exerciseManager.GetCodesAsync(WebsiteCulture, token);

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        static XElement Url(XNamespace ns, string path, DateTime? lastModified = null)
        {
            var url = new XElement(ns + "url", new XElement(ns + "loc", SiteUrl + path));
            if (lastModified.HasValue)
            {
                url.Add(new XElement(ns + "lastmod", lastModified.Value.ToString("yyyy-MM-dd")));
            }
            return url;
        }

        var urlSet = new XElement(ns + "urlset",
            StaticPages.Select(path => Url(ns, path)),
            blogs.Select(blog => Url(ns, $"/blogs/{blog.Id}", blog.Date)),
            exerciseCodes.Select(code => Url(ns, $"/exercise/details/{code}")));

        var xml = new XDocument(new XDeclaration("1.0", "utf-8", null), urlSet);
        return Content(xml.Declaration + Environment.NewLine + xml.Root, "application/xml; charset=utf-8");
    }
}
