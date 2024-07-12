using System.Device.Gpio;
using System.Numerics;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/io/{id}", (int id) =>
{
    var controller = new GpioController();
    controller.OpenPin(id, PinMode.Input);
    var status = controller.Read(id);
    return status;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();
