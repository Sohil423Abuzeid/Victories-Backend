using InstaHub.Models;
using InstaHub.Repositories;
using InstaHub.Services;
using InstaHub.Services.ChannelsServices.WhatsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddWebSocketManager();

builder.Services.AddHttpClient<IWhatsAppService, WhatsAppService>();
builder.Services.AddHttpClient<ITicketService, TicketService>();
builder.Services
    .AddScoped<IMessageService, MessageService>()
    .AddScoped<ITicketService, TicketService>()
    .AddScoped<IWhatsAppService, WhatsAppService>()
    .AddScoped<IMessageRepository, MessageRepository>()
    .AddScoped<ITicketRepository, TicketRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseWebSockets(); // Enable WebSocket support
var socketHandler = app.Services.GetService<MessageSocketHandler>();
app.MapWebSocketManager("/ws", socketHandler); // Define your WebSocket endpoint


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
