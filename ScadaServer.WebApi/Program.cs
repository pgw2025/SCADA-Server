using SqlSugar;
using ScadaServer.Application.Interfaces;
using ScadaServer.Application.Services;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.Repositories;
using ScadaServer.Infrastructure.Communication;
using ScadaServer.Infrastructure.Workers;
using ScadaServer.Infrastructure.Services;
using ScadaServer.Application.Options;
using ScadaServer.WebApi.Services;
using ScadaServer.WebApi.Hubs;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SystemDbOptions>(builder.Configuration.GetSection(SystemDbOptions.SectionName));
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                );

            var result = new
            {
                success = false,
                message = "数据校验失败",
                errors = errors
            };

            return new BadRequestObjectResult(result);
        };
    });

// Add CORS policy
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>() ?? Array.Empty<string>();
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins(allowedOrigins)
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// JWT Authentication Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_FOR_SCADA_SERVER_12345";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ScadaServer API", Version = "v1" });
    
    // 添加 JWT 认证支持
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

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
            new string[] {}
        }
    });
});

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
builder.Services.AddScoped<IAlarmRuleRepository, AlarmRuleRepository>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IConfigLogRepository, ConfigLogRepository>();
builder.Services.AddScoped<IDatabaseConfigRepository, DatabaseConfigRepository>();
builder.Services.AddScoped<IDataConversionRepository, DataConversionRepository>();
builder.Services.AddScoped<IDataModelRepository, DataModelRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IExposedInterfaceRepository, ExposedInterfaceRepository>();
builder.Services.AddScoped<IHistoricalRecordRepository, HistoricalRecordRepository>();
builder.Services.AddScoped<IHmiComponentRepository, HmiComponentRepository>();
builder.Services.AddScoped<IModelVariableRepository, ModelVariableRepository>();
builder.Services.AddScoped<IMqttServerRepository, MqttServerRepository>();
builder.Services.AddScoped<IRealtimeDataRepository, RealtimeDataRepository>();
builder.Services.AddScoped<IScadaPageRepository, ScadaPageRepository>();
builder.Services.AddScoped<IScadaProjectRepository, ScadaProjectRepository>();
builder.Services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
builder.Services.AddScoped<ISystemLogRepository, SystemLogRepository>();
builder.Services.AddScoped<ISystemScriptRepository, SystemScriptRepository>();
builder.Services.AddScoped<ISystemUserRepository, SystemUserRepository>();
builder.Services.AddScoped<IVariableTriggerRepository, VariableTriggerRepository>();

builder.Services.AddSingleton<DeviceRegistry>();
builder.Services.AddSingleton<ScadaServer.Infrastructure.Configuration.DatabaseConfigManager>();
builder.Services.AddSingleton<SystemMonitorService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<SystemMonitorService>());

// 4. Register Application Services
builder.Services.AddScoped<IAlarmRuleAppService, AlarmRuleAppService>();
builder.Services.AddScoped<IAreaAppService, AreaAppService>();
builder.Services.AddScoped<IConfigLogAppService, ConfigLogAppService>();
builder.Services.AddScoped<IDatabaseConfigAppService, DatabaseConfigAppService>();
builder.Services.AddScoped<IDataConversionAppService, DataConversionAppService>();
builder.Services.AddScoped<IDataModelAppService, DataModelAppService>();
builder.Services.AddScoped<IDeviceAppService, DeviceAppService>();
builder.Services.AddScoped<IExposedInterfaceAppService, ExposedInterfaceAppService>();
builder.Services.AddScoped<IHistoricalRecordAppService, HistoricalRecordAppService>();
builder.Services.AddScoped<IHmiComponentAppService, HmiComponentAppService>();
builder.Services.AddScoped<IModelVariableAppService, ModelVariableAppService>();
builder.Services.AddScoped<IMqttServerAppService, MqttServerAppService>();
builder.Services.AddScoped<IRealtimeDataAppService, RealtimeDataAppService>();
builder.Services.AddScoped<IScadaPageAppService, ScadaPageAppService>();
builder.Services.AddScoped<IScadaProjectAppService, ScadaProjectAppService>();
builder.Services.AddScoped<IScheduledTaskAppService, ScheduledTaskAppService>();
builder.Services.AddScoped<ISensorAppService, SensorAppService>();
builder.Services.AddScoped<ISystemConfigAppService, SystemConfigAppService>();
builder.Services.AddScoped<ISystemLogAppService, SystemLogAppService>();
builder.Services.AddScoped<ISystemScriptAppService, SystemScriptAppService>();
builder.Services.AddScoped<ISystemUserAppService, SystemUserAppService>();
builder.Services.AddScoped<IVariableTriggerAppService, VariableTriggerAppService>();

builder.Services.AddSingleton<IScadaNotificationService, SignalRNotificationService>();

// 5. Register Background Worker
builder.Services.AddHostedService<DeviceWorker>();

var app = builder.Build();

// 自动初始化数据库表结构
app.InitDatabase();

// Use Custom Global Exception Middleware
app.UseMiddleware<ScadaServer.WebApi.Middlewares.ExceptionMiddleware>();

// 始终启用 Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScadaServer API v1");
    c.RoutePrefix = string.Empty; // 这会让 Swagger 成为首页 (http://localhost:5043/)
});

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ScadaHub>("/hubs/scada");

app.Run();
