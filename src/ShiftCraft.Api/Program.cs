using System.Text;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShiftCraft.Api.Extensions;
using ShiftCraft.Api.Services;
using ShiftCraft.Application.Interfaces;
using ShiftCraft.Application.Services;
using ShiftCraft.Infrastructure.Data;
using ShiftCraft.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
#pragma warning disable CA1416
if (!builder.Environment.IsDevelopment() && OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog();
}
#pragma warning restore CA1416

builder.Services.AddDbContext<ShiftCraftDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEmployeeRoleRepository, EmployeeRoleRepository>();
builder.Services.AddScoped<IDayTypeRepository, DayTypeRepository>();
builder.Services.AddScoped<IShiftTemplateRepository, ShiftTemplateRepository>();
builder.Services.AddScoped<IWeeklyScheduleRepository, WeeklyScheduleRepository>();
builder.Services.AddScoped<IScheduleDayRepository, ScheduleDayRepository>();
builder.Services.AddScoped<IShiftAssignmentRepository, ShiftAssignmentRepository>();
builder.Services.AddScoped<IShiftRequirementRepository, ShiftRequirementRepository>();
builder.Services.AddScoped<IWorkRuleRepository, WorkRuleRepository>();
builder.Services.AddScoped<ICoreStaffRuleRepository, CoreStaffRuleRepository>();
builder.Services.AddScoped<IRuleViolationRepository, RuleViolationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILoginLogRepository, LoginLogRepository>();
builder.Services.AddScoped<IPublishLogRepository, PublishLogRepository>();
builder.Services.AddScoped<IDeviceRegistrationRepository, DeviceRegistrationRepository>();
builder.Services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

builder.Services.AddScoped<IRuleEngineService, RuleEngineService>();
builder.Services.AddScoped<IScheduleValidationService, ScheduleValidationService>();
builder.Services.AddScoped<IPushNotificationService, PushNotificationService>();

// JWT Authentication
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "ShiftCraft-Super-Secret-Key-2024-Minimum-32-Characters!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ShiftCraft";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ShiftCraftMobile";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ShiftCraft API", Version = "v1" });
});
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseGlobalExceptionHandler();

// Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShiftCraft API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Log startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("ShiftCraft API started. Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();