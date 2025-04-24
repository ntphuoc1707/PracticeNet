using Common;
using DB;
using MessageQueue;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Security;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using UserService.Services;
using static Utility.Common;
internal class Program
{

    private static JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Authentication
        builder.Services.AddJwtAuthentication();


        // Add services to the container.
        //DB
        builder.Services.AddDbContext<AppDbContext>();

        //RabbitMQ
        builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.AddSingleton(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
        builder.Services.AddHostedService<UserServiceConsumer>();

        builder.Services.AddControllers();

        //Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            // Add Bearer Auth Definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authentication",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid token.\n\nExample: `abcdef12345`"
            });

            // Add global security requirement
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {   
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
             });
        });

        //gRPC
        builder.Services.AddGrpc();

        //CORS
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

        // Hosting Service
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

        //Log
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.File(builder.Configuration.GetSection("LogFilePath").Value,
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
        builder.Host.UseSerilog();


        var app = builder.Build();

        //Migrate DB
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
        }


        app.UseCors("AllowGateway");
        app.MapGrpcService<GrpcProvider.GrpcProvider>();
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