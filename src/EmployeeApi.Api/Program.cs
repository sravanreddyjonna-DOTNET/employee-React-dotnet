using EmployeeApi.Api.Middleware;
using EmployeeApi.Application;
using EmployeeApi.Infrastructure;
using EmployeeApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string ReactAppCorsPolicy = "ReactApp";

// --- Services -----------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddCors(options =>
{
    // Matches the Vite dev server default port for the React app.
    options.AddPolicy(ReactAppCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- Pipeline -------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Dev convenience: apply any pending EF Core migrations on startup so you
    // don't need a separate "dotnet ef database update" step while iterating.
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(ReactAppCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
