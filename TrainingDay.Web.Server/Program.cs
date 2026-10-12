using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using TrainingDay.Web.Database;
using TrainingDay.Web.Server.Extensions;
using TrainingDay.Web.Services;

Serilog.Core.Logger logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .WriteTo.Console(outputTemplate:
    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File($"Logs/log_.log", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 1024 * 1024 * 50, outputTemplate:
    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    logger.Information($"Application starting");
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);

    var conString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<TrainingDayContext>(options => options.UseMySQL(conString));

    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(MyAllowSpecificOrigins, corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .SetIsOriginAllowed(_ => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddControllers();
    builder.Services.InstallServices(builder.Configuration, builder.Environment);

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
    builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key ?? string.Empty)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
    });

    builder.Services.AddApiVersioning();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Prerendered pages live in folders (e.g. blogs/19/index.html); serve them at /blogs/19 without redirecting to /blogs/19/.
    app.UseDefaultFiles(new DefaultFilesOptions { RedirectToAppendTrailingSlash = false });

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseCors(MyAllowSpecificOrigins);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapSpaFallback();

    using (var scope = app.Services.CreateScope())
    {
        TrainingDayContext db = scope.ServiceProvider.GetRequiredService<TrainingDayContext>();
        await db.Database.MigrateAsync();
        await ExercisesInitializer.InitializeAsync(db);

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }

    app.Run();
    logger.Information($"Application started");
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
