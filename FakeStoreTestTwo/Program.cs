using FakeStoreTwo.Data;
using FakeStoreTwo.Services.Interfaces;
using FakeStoreTwo.Services.Repositories;
using FakeStoreTwo.Services.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//to be added - MongoDB
var config = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(config);
builder.Services.AddSingleton<AppSettings>(sp=>sp.GetRequiredService<IOptions<AppSettings>>().Value);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//to be added - HttpClient and AddSingleton for the classes
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
