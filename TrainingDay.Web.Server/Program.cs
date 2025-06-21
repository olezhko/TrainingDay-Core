using Microsoft.EntityFrameworkCore;
using Serilog;
using TrainingDay.Web.Database;
using TrainingDay.Web.Server.Extensions;

var logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File($"Logs/log_.log", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 1024 * 1024 * 50, outputTemplate:
    "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);

    var conString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<TrainingDayContext>(options => options.UseMySQL(conString));

    builder.Services.AddControllers();
    builder.Services.InstallServices(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.UseCors("default");
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.MapFallbackToFile("/index.html");

    using (var scope = app.Services.CreateScope())
    {
        TrainingDayContext db = scope.ServiceProvider.GetRequiredService<TrainingDayContext>();
        db.Database.Migrate();
        ExercisesInitializer.Initialize(db).Wait();
    }

    app.Run();
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
