using Shared;
using ChatServer.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int userID = 0;


// TODO: Create helpers for adding, removing and getting users/user property contents from this dictionary
// Dictionary keyed by Username
var users = new Dictionary<string, User>
{
    { "Ducklord", new User { Id = userID++, Password = "chatking" } }
};

// Login
app.MapPost("/login", (LoginDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  if (users.TryGetValue(dto.Username, out var user) && user.Password == dto.Password)
  {
    return Results.Ok(new { UserID = user.Id, Message = "Login successful" });
  }

  return Results.BadRequest(new { Message = "Invalid username or password" });
});

// Registration
app.MapPost("/register", (LoginDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  // Check if username already exists
  if (users.ContainsKey(dto.Username))
  {
    return Results.BadRequest(new { Message = "Username already exists" });
  }

  // Create new userID
  userID++;

  users[dto.Username] = new User
  {
    Id = userID,
    Password = dto.Password
  };

  return Results.Ok(new { UserID = userID, Message = "Registration successful" });
});

app.MapGet("/users", () =>
{
  return Results.Ok(users.Keys.ToList());
});

// TODO: POST for message to chat

// TODO: GET all messages
app.Run();
