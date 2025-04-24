using LogService.Controllers;
using LogService.Interfaces;
using MessageQueue;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using static Utility.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//rabbitmq
var rabbitconfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.Configure<RabbitMQSetting>(rabbitconfig);

builder.Services.AddSingleton<ILogService, LogService.Services.LogService>();


builder.Services.AddHostedService<LogServiceConsumer>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(builder.Configuration.GetSection("LogFilePath").Value,
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();


var serverInfo = builder.Configuration.GetSection("ServerInfo");
builder.Services.Configure<ServerInfoSetting>(serverInfo);

var instanceServerInfo = serverInfo.Get<ServerInfoSetting>();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Any, instanceServerInfo.Port, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps(instanceServerInfo.HttpCertPath, instanceServerInfo.HttpCertPass);
    });
});
builder.Host.UseWindowsService();



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
