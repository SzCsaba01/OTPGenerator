using Microsoft.EntityFrameworkCore;
using OTPGenerator.API.Infrastructure;
using OTPGenerator.Data.Access.Data;
using Serilog;
using System.Net;
using User.API.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 7295, listenOptions =>
    {
        var certificatePath = builder.Configuration["Certificate:Path"];
        var certificatePassword = builder.Configuration["Certificate:Password"];
        listenOptions.UseHttps(certificatePath, certificatePassword);
    });
});

builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console() 
        .WriteTo.File("logs/app-log.txt", rollingInterval: RollingInterval.Day) 
        .Enrich.FromLogContext();
});

builder.Services.AddDbContext<OTPGeneratorContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddServices();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Origins");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.MapControllers();

app.Run();
