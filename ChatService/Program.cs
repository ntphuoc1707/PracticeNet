using ChatService.Hubs;
using ChatService.Services;
using DB;
using MessageQueue;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//RabbitMQ
builder.Services.Configure<RabbitMQSetting>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton(typeof(IRabbitMQPublisher<>), typeof(RabbitMQPublisher<>));
//builder.Services.AddHostedService<ChatServiceConsumer>();


//DB
builder.Services.AddDbContext<AppDbContext>();


builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapHub<ChatHub>("/chatHub");


app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
