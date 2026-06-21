using API_Dev_IT.Context;
using API_Dev_IT.Helper;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using API_Dev_IT.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<BookingContext>(option => option
                .UseSqlServer(builder.Configuration
                .GetConnectionString("Booking_Db")));

builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<IJwt, JwtService>();
builder.Services.AddScoped<IRoom, RoomService>();
builder.Services.AddScoped<ITenant, TenantService>();
builder.Services.AddScoped<IPayment, PaymentService>();
builder.Services.AddScoped<IBooking, BookingService>();
builder.Services.AddScoped<UserRoleHelper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebApi",
        Description = $"swagger api - ({builder.Environment.EnvironmentName})"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "open api",
        Type =  SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                     Id = "Bearer",
                      Type =  ReferenceType.SecurityScheme
                }
            },
            new List< string >()
        }
    });
});

var jwtSecretKey = builder.Configuration["JWT:SecretKey"];
var jwtIssuer = builder.Configuration["JWT:Issuer"];
var jwtAudience = builder.Configuration["JWT:Audience"];

if (string.IsNullOrWhiteSpace(jwtSecretKey))
{
    Log.Fatal("JWT:SecretKey is not configured");
    throw new InvalidOperationException("JWT:SecretKey is not configured. " +
                                        "Add it to appsettings.json or user secrets.");
}

if (Encoding.UTF8.GetBytes(jwtSecretKey).Length < 32)
{
    Log.Fatal("JWT:SecretKey must be at least 32 bytes (256 bits)");
    throw new InvalidOperationException("JWT:SecretKey must be at least 32 bytes (256 bits)." +
                                        "Generate a new key: openssl rand -base64 32");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer ?? "API_Dev_IT",
            ValidateAudience = true,
            ValidAudience = jwtAudience ?? "BookingClient",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Log.Warning(context.Exception,
                    $"JWT authentication failed for request {context.Request.Path}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Log.Debug($"JWT token validated for user {userId}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Log.Warning($"JWT challenge issued for {context.Request.Path} - {context.Error}");
                return Task.CompletedTask;
            }
        };
    });

var projectRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
var contentRoot = builder.Environment.ContentRootPath;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration as IConfiguration)
    .WriteTo.File(
        path: Path.Combine(contentRoot, "logs", "log-2026.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14)
    .WriteTo.File(
        path: Path.Combine(contentRoot, "logs", "errors-.txt"),
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        retainedFileCountLimit: 30)
    .CreateLogger();


builder.Services.AddCors(
    option => option.AddPolicy("FrontEnd",
    builder => builder.WithOrigins("http://localhost:4200")
                      .AllowCredentials()
                      .WithMethods("DELETE", "PUT", "POST", "GET", "OPTIONS")
                      .WithHeaders(
                            "Content-Type", 
                            "Authorization",
                            "X-Client-Data",
                            "X-csrf-Token",
                            "X-Requested-With",
                            "Accept")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", $"swagger api - " +
        $"({builder.Environment.EnvironmentName})"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("FrontEnd");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
