using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cache.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<CacheContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CacheContext") ?? throw new InvalidOperationException("Connection string 'CacheContext' not found.")));

var multiplexer = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Connection string 'Redis' not found."));
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();