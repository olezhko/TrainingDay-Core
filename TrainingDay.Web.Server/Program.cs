using Microsoft.EntityFrameworkCore;
using TrainingDay.Web.Database;
using TrainingDay.Web.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

var conString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TrainingDayContext>(options => options.UseSqlServer(conString));


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
    var db = scope.ServiceProvider.GetRequiredService<TrainingDayContext>();
    db.Database.Migrate();
    ExercisesInitializer.Initialize(db);
}

app.Run();
