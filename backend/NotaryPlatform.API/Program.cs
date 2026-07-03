using Asp.Versioning;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NotaryPlatform.API.Middleware;
using NotaryPlatform.Application;
using NotaryPlatform.Infrastructure;
using NotaryPlatform.Infrastructure.Persistence.Seed.Orchestration;

var envPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env"));
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

// Route all validation errors through GlobalExceptionMiddleware instead of
// ASP.NET Core's built-in ProblemDetails format.
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NotaryPlatform API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste a valid JWT access token.",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer",
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── Optional database seeding (opt-in; dev / staging only) ───────────────────
// Enable by setting Seeding__RunOnStartup=true in .env. Idempotent — each seeder
// skips rows that already exist. The profile comes from Seeding__Profile
// (default Development) and picks which seeders + volumes run.
// NEVER enable in production: the Development/Staging profiles seed fake
// tenants, users, and business data.
if (app.Configuration.GetValue<bool>("Seeding:RunOnStartup"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ISeedOrchestrator>().RunAsync();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
