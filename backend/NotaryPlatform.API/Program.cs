using DotNetEnv;
using NotaryPlatform.Infrastructure;

var envPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env"));
if (File.Exists(envPath))
{
	Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
