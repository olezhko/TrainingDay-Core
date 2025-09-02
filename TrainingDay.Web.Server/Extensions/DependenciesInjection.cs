using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using TrainingDay.Web.Data.Common.Files;
using TrainingDay.Web.Data.OpenAI;
using TrainingDay.Web.Data.Repositories;
using TrainingDay.Web.Server.Managers;
using TrainingDay.Web.Services;
using TrainingDay.Web.Services.Blogs;
using TrainingDay.Web.Services.Email;
using TrainingDay.Web.Services.Exercises;
using TrainingDay.Web.Services.Firebase;
using TrainingDay.Web.Services.OpenAI;
using TrainingDay.Web.Services.Repositories;
using TrainingDay.Web.Services.Support;
using TrainingDay.Web.Services.UserTokens;
using TrainingDay.Web.Services.YoutubeVideo;

namespace TrainingDay.Web.Server.Extensions
{
    public static class DependenciesInjection
    {
        public static void InstallServices(this IServiceCollection services, ConfigurationManager configuration, Serilog.Core.Logger logger)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("default", corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.Configure<OpenAISettings>(configuration.GetSection("OpenAI"));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IYoutubeVideoCatalog, YoutubeVideoCatalog>();
            services.AddScoped<IExerciseManager, ExerciseManager>();
            services.AddScoped<IUserTokenManager, UserTokenManager>();
            services.AddScoped<IBlogPostsManager, BlogPostsManager>();
            services.AddScoped<IFirebaseService, FirebaseService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddSingleton<IOpenAIService, OpenAIService>();
            //services.AddScoped<IMessageProducer, RabbitMQProducer>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISupportManager, SupportManager>();

            services.AddTransient<ISupportRepository, SupportRepository>();

            string GetPathToSettingsFile(string settingsFileName)
            {
                var customFile = "Settings" + Path.DirectorySeparatorChar + settingsFileName;
                if (File.Exists(customFile))
                {
                    logger.Information("firebase file exists");
                    return customFile;
                }

                return "Settings" + Path.DirectorySeparatorChar + settingsFileName;
            }

            var firebaseFilePath = GetPathToSettingsFile("firebase.json");

            logger.Information($"Try to load firebase file: {firebaseFilePath}, CurrentDirectory: {Environment.CurrentDirectory}");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(firebaseFilePath)
            });

            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
        }
    }
}
