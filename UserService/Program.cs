using Azure.Core;
using Common;
using DB;
using MessageQueue;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Security;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using UserService.Services;
internal class Program
{

    private static JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddJwtAuthentication();

        var rabbitconfig = builder.Configuration.GetSection("RabbitMQ");

        //IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        // Add services to the container.
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.Configure<RabbitMQSetting>(rabbitconfig);
        builder.Services.AddSingleton(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));

        builder.Services.AddHostedService<UserServiceConsumer>();

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddGrpc();
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
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Listen(System.Net.IPAddress.Any, 4444, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                listenOptions.UseHttps("G:\\MyFirstCert.pfx", "phuoc123");
            });
        });
        builder.Host.UseWindowsService();
        Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(builder.Configuration.GetSection("LogFilePath").Value,
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

        builder.Host.UseSerilog();
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }
        app.UseCors("AllowGateway");
        app.MapGrpcService<GrpcProvider.GrpcProvider>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
           //builder.Services.AddCors(options =>
           //{
           //    options.AddPolicy("AllowAll",
           //        policy =>
           //        {
           //            policy.AllowAnyOrigin()
           //                  .AllowAnyMethod()
           //                  .AllowAnyHeader();
           //        });
           //});
           //app.UseCors("AllowAll");
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
        }
        app.UseMiddleware<CustomException>();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}