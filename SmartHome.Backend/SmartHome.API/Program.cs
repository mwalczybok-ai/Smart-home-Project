using SmartHome.API.Models;
using SmartHome.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HubSettings>(builder.Configuration.GetSection("Hub"));
builder.Services.Configure<CosmoDbSettings>(builder.Configuration.GetSection("Cosmos"));

builder.Services.AddSingleton<SmartHomeDb>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();