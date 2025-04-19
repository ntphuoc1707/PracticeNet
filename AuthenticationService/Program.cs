using Common;
using MessageQueue;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using static GrpcProvider.Protos.GrpcProvider;

var builder = WebApplication.CreateBuilder(args);
IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
// Add services to the container.
builder.Services.Configure<RabbitMQSetting>(config.GetSection("RabbitMQ"));
builder.Services.AddSingleton(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<GrpcProviderClient>(o =>
{
    o.Address = new Uri(builder.Configuration.GetSection("UserServiceIP").Value);
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(config.GetSection("LogFilePath").Value,
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Any, 3333, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        listenOptions.UseHttps("G:\\MyFirstCert.pfx", "phuoc123");
    });
});
builder.Host.UseWindowsService();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGateway",
        policy =>
        {
            policy.WithOrigins("https://localhost:5001")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
var app = builder.Build();


app.UseCors("AllowGateway");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseMiddleware<CustomException>();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
