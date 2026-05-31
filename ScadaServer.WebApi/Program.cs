using SqlSugar;
using ScadaServer.Application.Interfaces;
using ScadaServer.Application.Services;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.Workers;
using ScadaServer.Application.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SystemDbOptions>(builder.Configuration.GetSection(SystemDbOptions.SectionName));
builder.Services.AddControllers();
// ...

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Register SqlSugar (Scoped)
builder.Services.AddScoped<ISqlSugarClient>(s =>
{
    var options = s.GetRequiredService<IOptions<SystemDbOptions>>().Value;
    return new SqlSugarScope(new ConnectionConfig()
    {
        ConnectionString = options.GetConnectionString(),
        DbType = DbType.MySql, 
        IsAutoCloseConnection = true
    });
});

// 2. Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, SqlSugarUnitOfWork>();

// 3. Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(SqlSugarRepository<>));
builder.Services.AddScoped<IDeviceRepository, SqlSugarDeviceRepository>();
builder.Services.AddSingleton<ScadaServer.Infrastructure.Configuration.DatabaseConfigManager>();

// 4. Register Application Services
builder.Services.AddScoped<IDeviceAppService, DeviceAppService>();
builder.Services.AddSingleton<IMqttService, MqttHandler>();

// 5. Register Background Worker
builder.Services.AddHostedService<DeviceWorker>();

var app = builder.Build();

// Use Custom Global Exception Middleware
app.UseMiddleware<ScadaServer.WebApi.Middlewares.ExceptionMiddleware>();

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
