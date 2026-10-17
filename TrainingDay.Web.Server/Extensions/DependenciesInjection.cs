using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Data.Repositories;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Services;
using TrainingDay.Web.Services.Auth;
using TrainingDay.Web.Services.Blogs;
using TrainingDay.Web.Services.DeepSeek;
using TrainingDay.Web.Services.Email;
using TrainingDay.Web.Services.Exercises;
using TrainingDay.Web.Services.Extensions;
using TrainingDay.Web.Services.Firebase;
using TrainingDay.Web.Services.Notification;
using TrainingDay.Web.Services.OpenAI;
using TrainingDay.Web.Services.Rabbit;
using TrainingDay.Web.Services.Repositories;
using TrainingDay.Web.Services.SocialWorkouts;
using TrainingDay.Web.Services.Support;
using TrainingDay.Web.Services.UserRepo;
using TrainingDay.Web.Services.UserTokens;
using TrainingDay.Web.Services.YoutubeVideo;

namespace TrainingDay.Web.Server.Extensions;

public static class DependenciesInjection
{
    public static void InstallServices(this IServiceCollection services, ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        services.AddIdentity<MobileUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<TrainingDayContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            // Dev: Angular (http) → backend (https) are cross-scheme, so SameSite=Lax blocks the cookie.
            // SameSite=None + Secure lets the browser send it cross-site in dev.
            options.Cookie.SameSite = environment.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Lax;
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.SlidingExpiration = true;

            options.Events.OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                else
                    ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                else
                    ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
        });

        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<AiOptions>(configuration.GetSection("AI"));
        services.Configure<OpenAIOptions>(configuration.GetSection("OpenAI"));
        services.Configure<DeepSeekOptions>(configuration.GetSection("DeepSeek"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddTransient<IYoutubeVideoCatalog, YoutubeVideoCatalog>();
        services.AddScoped<IExerciseManager, ExerciseManager>();
        services.AddScoped<IUserTokenManager, UserTokenManager>();
        services.AddScoped<IBlogPostsManager, BlogPostsManager>();
        services.AddScoped<ISocialWorkoutManager, SocialWorkoutManager>();
        services.AddScoped<IUserRepoManager, UserRepoManager>();
        services.AddScoped<IFirebaseService, FirebaseService>();
        services.AddScoped<IStringLocalizer, StringLocalizerService>();
        // The provider is read per resolution, so switching "AI:Provider" in appsettings takes effect without a restart.
        services.AddSingleton<OpenAIService>();
        services.AddSingleton<DeepSeekService>();
        services.AddTransient<IAiService>(sp => sp.GetRequiredService<IOptionsMonitor<AiOptions>>().CurrentValue.Provider switch
        {
            AiProvider.DeepSeek => sp.GetRequiredService<DeepSeekService>(),
            _ => sp.GetRequiredService<OpenAIService>()
        });
        services.AddScoped<IMessageProducer, RabbitMQProducer>();

        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<ISupportManager, SupportManager>();

        services.AddTransient<ISupportRepository, SupportRepository>();

        string GetPathToSettingsFile(string settingsFileName)
        {
            var customFile = "Settings" + Path.DirectorySeparatorChar + settingsFileName;
            if (File.Exists(customFile))
            {
                return customFile;
            }

            return "Settings" + Path.DirectorySeparatorChar + settingsFileName;
        }

        Stream GetStream(string settingsFileName)
        {
            return File.OpenRead(settingsFileName);
        }

        var firebaseFilePath = GetPathToSettingsFile("firebase.json");

        FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromStream(GetStream(firebaseFilePath))
        });

        services.AddHostedService<ConsumeScopedServiceHostedService>();
        services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
    }
}
