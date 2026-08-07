using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainingDay.Web.Entities;
using TrainingDay.Web.Entities.MobileItems;

namespace TrainingDay.Web.Database;

public class TrainingDayContext : IdentityDbContext<MobileUser, IdentityRole<Guid>, Guid>
{
    public DbSet<BlogPost> Posts { get; set; }
    public DbSet<BlogPostCulture> PostCultures { get; set; }
    public DbSet<Culture> Cultures { get; set; }

    public DbSet<SupportRequest> SupportRequests { get; set; }

    public DbSet<WebExercise> Exercises { get; set; }
    public DbSet<ExerciseVideoLink> ExerciseVideoLinks { get; set; }

    public DbSet<MobileToken> MobileTokens { get; set; }
    public DbSet<MobileUser> MobileUsers { get; set; }
    public DbSet<UserMobileToken> UserTokens { get; set; }


    public DbSet<UserTrainingGroup> UserTrainingGroups { get; set; }
    public DbSet<UserLastTraining> UserLastTrainings { get; set; }
    public DbSet<UserLastTrainingExercise> UserLastTrainingExercises { get; set; }
    public DbSet<UserWeightNote> UserWeightNotes { get; set; }
    public DbSet<UserTraining> UserTrainings { get; set; }
    public DbSet<UserSuperSet> UserSuperSets { get; set; }
    public DbSet<UserTrainingExercise> UserTrainingExercises { get; set; }
    public DbSet<UserExercise> UserExercises { get; set; }

    public DbSet<SocialWorkout> SocialWorkouts { get; set; }
    public DbSet<SocialWorkoutExercise> SocialWorkoutExercises { get; set; }
    public DbSet<SocialWorkoutLike> SocialWorkoutLikes { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public TrainingDayContext(DbContextOptions<TrainingDayContext> options)
    : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditSaveChangesInterceptor());
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MobileUser>().ToTable("MobileUsers");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("AspNetRoles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens");

        modelBuilder.Entity<Culture>().HasData(
            new() { Id = 1, Name = "Русский", Code = "ru" },
            new() { Id = 2, Name = "English", Code = "en" });

        modelBuilder.Entity<MobileUser>()
            .Property(item => item.ShareCompletedWorkouts)
            .HasDefaultValue(true);

        modelBuilder.Entity<SocialWorkoutLike>()
            .HasIndex(item => new { item.SocialWorkoutId, item.UserId })
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(item => item.Token)
            .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasOne(item => item.User)
            .WithMany(item => item.RefreshTokens)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}