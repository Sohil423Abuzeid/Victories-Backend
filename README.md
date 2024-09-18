# InstaHub Multi-Channel Messaging API Backend

## Overview

This backend API provides functionality for managing messages, tickets, customer data, and admin operations for businesses using multiple communication channels (WhatsApp, Instagram, etc.). It includes features for ticket prioritization, sentiment analysis, AI-driven ticket updates, real-time notifications, and more.

### Key Features
- **Login & Registration**
- **Ticket Management** with AI-powered updates
- **Get Similar Tickets Based on Summary & Category** with AI
- **Customer & Admin Profiles**
- **Real-Time Messaging**
- **Sentiment Analysis**
- **Customizable for Different Business Types**

## Endpoints

### Authentication
- **Login (Admin)**
  - `POST /api/admin/login`
  - Authenticate an admin user.

- **Register (Company)**
  - `POST /api/company/register`
  - Register a new company in the system.

### Home Page
- **Get Admins**
  - `GET /api/admin/admins`
  - Returns a list of admins with details: `Name`, `#Tickets`, and `Id`.

### Owner Page
- **Get Tickets by Admin ID**
  - `GET /api/tickets/{adminId}`
  - Retrieves tickets for a specific admin, including:
    - `Urgency`, `Status`, `Admin`, `Date`, `Sentiment Analysis`, `Sentiment Degree`, `Title`, and `Category`.

### Ticket Chat Page
- **Get Ticket Details by ID**
  - `GET /api/ticket/{ticketId}`
  - Returns details of a specific ticket:
    - `Id`, `Label`, `Summary`, `Category`, `Status`, `Customer Contact Way`, `Message Date`.

- **Send Message**
  - `POST /api/ticket/{ticketId}/message`
  - Sends a message related to a specific ticket. The message includes:
    - `Id`, `Content`, `SenderId`.

- **Receive Messages**
  - `GET /api/ticket/{ticketId}/messages`
  - Retrieves new messages and updates the front-end. Includes a mechanism for notifying the front-end to refresh the screen.

- **Update Ticket (AI-driven)**
  - `PUT /api/ticket/{ticketId}/update`
  - Updates ticket details using AI for:
    - `Category`, `Status`, `Summary`.

- **Get Message History by Ticket ID**
  - `GET /api/ticket/{ticketId}/history`
  - Retrieves the message history for a specific ticket.

### Search Tickets Popup
- **Get Similar Tickets**
  - `GET /api/ticket/{ticketId}/similar`
  - Retrieves the 5 most similar tickets to help admins.

### Customer Page
- **Get Customer**
  - `GET /api/customer/{customerId}`
  - Retrieves customer details: `Id`, `Contact Way`, `Registration Date`, `First Ticket Date`.

- **Get Tickets by Customer ID**
  - `GET /api/customer/{customerId}/tickets`
  - Retrieves tickets associated with a specific customer.

### Owner Profile
- **Get Owner**
  - `GET /api/owner`
  - Returns owner details: `Photo`, `First Name`, `Last Name`, `Email`.

- **Update Owner Image**
  - `PUT /api/owner/image`
  - Updates the owner’s profile image.

- **Delete Owner Image**
  - `DELETE /api/owner/image`
  - Deletes the owner’s profile image.

- **Update Owner Information**
  - `PUT /api/owner`
  - Updates owner details like `First Name`, `Last Name`, `Email`.

- **Update Owner Password**
  - `PUT /api/owner/password`
  - Updates the owner's password.

### Admin Management
- **Delete Admin by ID**
  - `DELETE /api/admin/{adminId}`
  - Deletes an admin by their `Id`.

- **Add Another Admin**
  - `POST /api/admin`
  - Adds a new admin with details like:
    - `First Name`, `Last Name`, `Phone Number`, `Email`, `Password`, `Username`.

- **Get Admin by ID**
  - `GET /api/admin/{adminId}`
  - Returns admin details: `Name`, `Registration Date`, `Id`, `#Tickets`, and list of `Tickets`.

### Pricing
- (Not Yet Implemented)
  - Future features will include pricing functionality.
### Statistics 
- (Not Yet Implemented)
   - Future Feature
## Technologies Used
- **.NET 8**
- **Entity Framework Core** (for database operations)
- **SignalR** (for real-time notifications)
- **AI Integration** (for ticket updates and sentiment analysis)

## Setup Instructions

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/SmartSparks-Victoris/Victories-Backend.git
   cd Victories-Backend
   ```

2. **Install Dependencies:**
   Ensure you have the .NET SDK installed, then run:
   ```bash
   dotnet restore
   ```

3. **Database Setup:**
   Use EF Core migrations to set up the database:
   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

5. **API Testing:**
   You can test the endpoints using tools like Postman or Swagger (if integrated).
