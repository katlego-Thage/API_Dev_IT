using API_Dev_IT.Context;
using API_Dev_IT.Helper;
using API_Dev_IT.IService;
using API_Dev_IT.Model;
using API_Dev_IT.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo
    {
        Version = "1",
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidAudience = builder.Configuration["JWT:Audiance"]??string.Empty,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                                    builder.Configuration["JWT:SecreteKey"]??string.Empty))
        };
});

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/V1/swagger.json", $"swagger api - " +
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

app.Run();
