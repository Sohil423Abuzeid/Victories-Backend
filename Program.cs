using InstaHub.Models;
using InstaHub.Repositories;
using InstaHub.Services;
using InstaHub.Services.Authentication;
using InstaHub.Services.ChannelsServices.WhatsAppServiceInfra;
using InstaHub.Services.ChannelsServices.WhatsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Services (Dependency Injection)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWebSocketManager();

builder.Services.Configure<WhatsAppSettings>(builder.Configuration.GetSection("WhatsAppSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();
builder.Services.AddHttpClient<ITicketService, TicketService>();

builder.Services
    .AddScoped<IMessageService, MessageService>()
    .AddScoped<ITicketService, TicketService>()
    .AddScoped<IWhatsAppService, WhatsAppService>();

builder.Services
    .AddScoped<IMessageRepository, MessageRepository>()
    .AddScoped<ITicketRepository, TicketRepository>();

builder.Services
    .AddScoped<IAdminService, AdminService>()
    .AddScoped<IOwnerService, OwnerService>()
    .AddScoped<IAuthService, AuthService>();

// Configure DbContext with connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// 2. Middleware Configuration

// Enable Swagger (API documentation)
app.UseSwagger();
app.UseSwaggerUI();

// Enable WebSocket support
app.UseWebSockets();

// Configure WebSocket handler endpoint
var socketHandler = app.Services.GetService<MessageSocketHandler>();
app.MapWebSocketManager("/ws", socketHandler); // Define WebSocket endpoint

// Configure HTTPS redirection (redirect HTTP requests to HTTPS)
app.UseHttpsRedirection();

// Enable Routing
app.UseRouting();

// Enable Authorization
app.UseAuthorization();

// Map default route for root URL
app.MapGet("/", () => "Hello World");

// Map controller endpoints
app.MapControllers();

app.Run();
