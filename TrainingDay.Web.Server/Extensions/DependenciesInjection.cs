using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using TrainingDay.Web.Database;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Data.Common.Files;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Data.Repositories;
using TrainingDay.Web.Server.Managers;
using TrainingDay.Web.Services;
using TrainingDay.Web.Services.Blogs;
using TrainingDay.Web.Services.Email;
using TrainingDay.Web.Services.Exercises;
using TrainingDay.Web.Services.Extensions;
using TrainingDay.Web.Services.Firebase;
using TrainingDay.Web.Services.Notification;
using TrainingDay.Web.Services.OpenAI;
using TrainingDay.Web.Services.Rabbit;
using TrainingDay.Web.Services.Repositories;
using TrainingDay.Web.Services.Support;
using TrainingDay.Web.Services.UserTokens;
using TrainingDay.Web.Services.YoutubeVideo;

namespace TrainingDay.Web.Server.Extensions
{
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
            services.Configure<OpenAIOptions>(configuration.GetSection("OpenAI"));

            services.AddTransient<IYoutubeVideoCatalog, YoutubeVideoCatalog>();
            services.AddScoped<IExerciseManager, ExerciseManager>();
            services.AddScoped<IUserTokenManager, UserTokenManager>();
            services.AddScoped<IBlogPostsManager, BlogPostsManager>();
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IStringLocalizer, StringLocalizerService>();
            services.AddSingleton<IOpenAIService, OpenAIService>();
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
}
