using Ocelot.DependencyInjection;
using Ocelot.Middleware;


var builder = WebApplication.CreateBuilder(args);


//Ocelot
IConfiguration config = new ConfigurationBuilder().AddJsonFile("ocelot.json").Build();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot(config);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOcelot();

app.MapControllers();

app.Run();
