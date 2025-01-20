using DotNetEnv;
using exam.api.Data;
using exam.api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables
Env.Load();

// Add services to the container
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Register dependencies
builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();