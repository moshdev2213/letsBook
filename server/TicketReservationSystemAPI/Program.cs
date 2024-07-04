// File name: Program.cs
// <summary>
// Description: Start up file for the web API
// </summary>
// <author> MulithaBM </author>
// <created>22/09/2023</created>
// <modified>11/10/2023</modified>

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using TicketReservationSystemAPI.Data;
using TicketReservationSystemAPI.Services.AdminService;
using TicketReservationSystemAPI.Services.AgentService;
using TicketReservationSystemAPI.Services.TravelerService;

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration.AddUserSecrets<Program>();

// Configure the database settings
builder.Services.Configure<DbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Configuration.GetSection("AppSettings").Get<JWTSettings>();

builder.Services.AddSingleton(provider =>
{
    var options = provider.GetService<IOptions<DbSettings>>() ?? throw new InvalidOperationException("MongoDbSettings is not configured properly");
    return new DataContext(options);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    c =>
    {
        c.AddSecurityDefinition("oauth2",
        new OpenApiSecurityScheme
        {
            Description = "Standard Authorization header using the Bearer scheme, e.g. \"bearer {token}\"",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
        });

        c.OperationFilter<SecurityRequirementsOperationFilter>();
    }
);

// CORS configurations
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .WithOrigins("http://localhost:5082")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Back-Office services
builder.Services.AddSingleton<IAdminService, AdminService>();
builder.Services.AddSingleton<IAdminTravelerService, AdminTravelerService>();
builder.Services.AddSingleton<IAdminTrainService, AdminTrainService>();
builder.Services.AddSingleton<IAdminReservationService, AdminReservationService>();

// Travel agent services
builder.Services.AddSingleton<IAgentService, AgentService>();
builder.Services.AddSingleton<IAgentTravelerService, AgentTravelerService>();
builder.Services.AddSingleton<IAgentTrainService, AgentTrainService>();
builder.Services.AddSingleton<IAgentReservationService, AgentReservationService>();

// Traveler services
builder.Services.AddSingleton<ITravelerService, TravelerService>();
builder.Services.AddSingleton<ITravelerTrainService, TravelerTrainService>();
builder.Services.AddSingleton<ITravelerReservationService, TravelerReservationService>();

string token = builder.Configuration["AppSettings:Token"] ?? throw new NullReferenceException("Missing token");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8
                .GetBytes(token)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }
);

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
        context.Response.StatusCode = 200;
    }
    else
    {
        await next();
    }
});

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
