using TrainingDay.Web.Data.BlogPosts;
using Xunit;

namespace TrainingDay.Web.Services.Tests.Blogs;

public class BlogsCultureTypesTests
{
    // Production blog posts, the website and the mobile app use 1 = English, 2 = Russian.
    [Theory]
    [InlineData(1, "en")]
    [InlineData(2, "ru")]
    public void ToLanguageCode_MatchesStoredBlogCultureIds(int cultureId, string expected)
    {
        Assert.Equal(expected, ((BlogsCultureTypes)cultureId).ToLanguageCode());
    }

    [Fact]
    public void ToLanguageCode_UnknownCulture_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ((BlogsCultureTypes)99).ToLanguageCode());
    }
}
