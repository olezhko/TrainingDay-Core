using System.ComponentModel;

namespace TrainingDay.Web.Data.BlogPosts;

/// <summary>
/// Language of a blog post, stored in <c>PostCultures.CultureId</c>.
/// Blog posts use 1 = English, 2 = Russian, the reverse of the <c>Cultures</c> table (1 = ru, 2 = en).
/// The website, the prerender script and the mobile app all send these ids, so they must not change.
/// </summary>
public enum BlogsCultureTypes
{
    [Description("English")]
    English = 1,
    [Description("Русский")]
    Russian = 2,
}

public static class BlogsCultureTypesExtensions
{
    /// <summary>Two-letter language code, as stored in <c>MobileTokens.Language</c>.</summary>
    public static string ToLanguageCode(this BlogsCultureTypes culture) => culture switch
    {
        BlogsCultureTypes.English => "en",
        BlogsCultureTypes.Russian => "ru",
        _ => throw new ArgumentOutOfRangeException(nameof(culture), culture, null),
    };
}
