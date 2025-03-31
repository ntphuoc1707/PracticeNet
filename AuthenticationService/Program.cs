using MessageQueue;
using System.Net;
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
    o.Address = new Uri("https://localhost:4444");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
