using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.Run();
