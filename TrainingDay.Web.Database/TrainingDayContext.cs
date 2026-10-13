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

        // Shared author for workouts shared by non-logged-in users. No password, so nobody can sign in as it.
        modelBuilder.Entity<MobileUser>().HasData(new MobileUser
        {
            Id = MobileUser.AnonymousUserId,
            UserName = MobileUser.AnonymousUserName,
            NormalizedUserName = MobileUser.AnonymousUserName.ToUpperInvariant(),
            SecurityStamp = "7F3B2C9E-5A41-4D8E-9C6B-1E2F3A4B5C6D",
            ConcurrencyStamp = "0B1C2D3E-4F50-4617-8293-A4B5C6D7E8F9",
            LockoutEnabled = true,
            ShareCompletedWorkouts = true,
        });

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