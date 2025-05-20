using Microsoft.EntityFrameworkCore;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Database;

public class TrainingDayContext : DbContext
{
    public DbSet<BlogPost> Posts { get; set; }
    public DbSet<BlogPostCulture> PostCultures { get; set; }
    public DbSet<Culture> Cultures { get; set; }


    public DbSet<WebExercise> Exercises { get; set; }
    public DbSet<SupportRequest> SupportRequests { get; set; }

    public DbSet<YoutubeVideoUrls> YoutubeVideoUrls { get; set; }


    public DbSet<UserTrainingGroup> UserTrainingGroups { get; set; }
    public DbSet<UserLastTraining> UserLastTrainings { get; set; }
    public DbSet<UserLastTrainingExercise> UserLastTrainingExercises { get; set; }
    public DbSet<UserWeightNote> UserWeightNotes { get; set; }
    public DbSet<UserTraining> UserTrainings { get; set; }
    public DbSet<UserSuperSet> UserSuperSets { get; set; }
    public DbSet<UserTrainingExercise> UserTrainingExercises { get; set; }
    public DbSet<UserExercise> UserExercises { get; set; }


    public DbSet<MobileToken> MobileTokens { get; set; }
    public DbSet<UserMobileToken> UserTokens { get; set; }
    public DbSet<User> Users { get; set; }

    public TrainingDayContext(DbContextOptions<TrainingDayContext> options)
    : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Culture>().HasData(
            new() { Id = 1, Name = "Русский", Code = "ru" },
            new() { Id = 2, Name = "English", Code = "en" });
    }
}