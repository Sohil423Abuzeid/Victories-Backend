using Microsoft.AspNetCore.Mvc;
using InstaHub.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class FakeDataController : ControllerBase
{
    private readonly AppDbContext _context;

    public FakeDataController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("fill")]
    public async Task<IActionResult> FillDatabase()
    {
        // Sample Admin data
        var admins = new[]
        {
            new Admin { UserName = "admin1", FirstName = "Alice", LastName = "Smith", PhoneNumber = "1234567890", Email = "alice@example.com", HashPassword = "hashedpassword1", RegisterationDate = DateTime.UtcNow },
            new Admin { UserName = "admin2", FirstName = "Bob", LastName = "Johnson", PhoneNumber = "0987654321", Email = "bob@example.com", HashPassword = "hashedpassword2", RegisterationDate = DateTime.UtcNow }
        };

        // Sample Category data
        var categories = new[]
        {
            new Category { Name = "Technical Support" },
            new Category { Name = "Billing" },
            new Category { Name = "General Inquiry" }
        };

        // Sample Customer data
        var customers = new[]
        {
            new Customer { CustomerId = "1", FirstTicketDate = DateTime.UtcNow },
            new Customer { CustomerId = "2", FirstTicketDate = DateTime.UtcNow }
        };

        // Sample Ticket data
        var tickets = new[]
        {
            new Ticket { CustomerId = customers[0].CustomerId, Category = categories[0], AdminsId = {admins[0].Id }, SentimentAnalysis = "B", Rate = 5, CreatedAt = DateTime.UtcNow, ClosedAt = DateTime.UtcNow.AddDays(1), Label = "Issue", StateId = 1, State = "Open", Summary = "Technical issue with product", Urgent = false },
            new Ticket { CustomerId = customers[1].CustomerId, Category = categories[1], AdminsId = {admins[1].Id }, SentimentAnalysis = "A", Rate = 4, CreatedAt = DateTime.UtcNow, ClosedAt = DateTime.UtcNow.AddDays(2), Label = "Billing", StateId = 1, State = "Open", Summary = "Question about billing", Urgent = false }
        };

        // Sample Message data
        var WhatsAppMessage = new[]
        {
            new WhatsAppMessage { MessageId = Guid.NewGuid().ToString(), TicketId = 1, SendDate = DateTime.UtcNow, ReceiveDate = DateTime.UtcNow.AddMinutes(1), TimeStamp = DateTime.UtcNow.ToString(), MessagingProduct = "Email", sent = true, MessageBody = "Thank you for reaching out!" },
            new WhatsAppMessage { MessageId = Guid.NewGuid().ToString(), TicketId = 2, SendDate = DateTime.UtcNow, ReceiveDate = DateTime.UtcNow.AddMinutes(2), TimeStamp = DateTime.UtcNow.ToString(), MessagingProduct = "Chat", sent = false, MessageBody = "Please clarify your billing question." }
        };

        // Add data to the database
        await _context.Admins.AddRangeAsync(admins);
        await _context.Categories.AddRangeAsync(categories);
        await _context.Customers.AddRangeAsync(customers);
        await _context.Tickets.AddRangeAsync(tickets);
        await _context.WhatsAppMessages.AddRangeAsync(WhatsAppMessage);

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Sample data added successfully!" });
    }
}
