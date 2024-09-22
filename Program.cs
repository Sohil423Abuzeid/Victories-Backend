using InstaHub.Models;
using InstaHub.Repositories;
using InstaHub.Services;
using InstaHub.Services.Authentication;
using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;
using InstaHub.Services.ChannelsServices.WhatsService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Services (Dependency Injection)

// Configure DbContext with connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register WebSocket manager
builder.Services.AddWebSocketManager();

// Configure WhatsApp settings and JWT settings
builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));

// Register HttpClient services
builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();
builder.Services.AddHttpClient<ITicketService, TicketService>();

// Register application services and repositories
builder.Services
    .AddScoped<IMessageService, MessageService>()
    .AddScoped<ITicketService, TicketService>()
    .AddScoped<IWhatsAppService, WhatsAppService>()

    .AddScoped<IMessageRepository, MessageRepository>()
    .AddScoped<ITicketRepository, TicketRepository>()

    .AddScoped<IAdminService, AdminService>()
    .AddScoped<IOwnerService, OwnerService>()
    .AddScoped<IAuthService, AuthService>();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtSecretFromConfig = jwtSettings["JwtSecret"];

var JwtSecret = Encoding.UTF8.GetBytes(jwtSecretFromConfig);

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
        IssuerSigningKey = new SymmetricSecurityKey(JwtSecret),
        ValidIssuer = jwtSettings["Issuer"], 
        ValidAudience = jwtSettings["Audience"], 
    };

    // event handlers for debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated: " + context.SecurityToken);
            return Task.CompletedTask;
        }
    };
});


// Authorization policy for Owner only
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OwnerOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Owner"));
});

var app = builder.Build();

// 2. Middleware Configuration

// Enable Swagger (API documentation)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        options.RoutePrefix = string.Empty;
    });
}

// Enable WebSocket support
app.UseWebSockets();

// Configure WebSocket handler endpoint
var socketHandler = app.Services.GetService<MessageSocketHandler>();
app.MapWebSocketManager("/ws", socketHandler); // Define WebSocket endpoint

// Configure HTTPS redirection (redirect HTTP requests to HTTPS)
app.UseHttpsRedirection();

// Enable Authentication Middleware
app.UseAuthentication();

// Enable Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable Routing
app.UseRouting();

// Map default route for root URL
app.MapGet("/", () => "Hello World");

// Map controller endpoints
app.MapControllers();

app.Run();