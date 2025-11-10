using Shared;
using ChatServer.Store;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Create a single shared UserStore instance.
// The store contains both dictionaries (by username and by id).
UserStore userStore = new();

// Create a single shared MessageStore instance
// The store contains both a list and dictionary (for ID lookup).
// It requires a reference to the UserStore to validate senders.
MessageStore messageStore = new(userStore);

// Add one user for testing
userStore.Add("Ducklord", "chatking");


// Login endpoint
app.MapPost("/login", (LoginDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  var user = userStore.GetByUsername(dto.Username);

  if (user != null && user.Password == dto.Password)
  {
    return Results.Ok(new { UserID = user.Id, Message = "Login successful" });
  }

  return Results.BadRequest(new { Message = "Invalid username or password" });
});


// Registration endpoint
app.MapPost("/register", (LoginDTO dto) =>
{
  // Validate input
  if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
  {
    return Results.BadRequest(new { Message = "Username and password are required" });
  }

  // Attempt to add user
  if (!userStore.Add(dto.Username, dto.Password))
  {
    return Results.BadRequest(new { Message = "Username already exists" });
  }

  var newUser = userStore.GetByUsername(dto.Username);
  if (newUser == null)
  {
    return Results.BadRequest(new { Message = "Failed to add user" });
  }

  return Results.Ok(new { UserID = newUser.Id, Message = "Registration successful" }); // TODO: refactor the Add() in UserStore to return the User object, avoiding the issue here.
});


// Return a list of all usernames (Note: only return the usernames, not the full objects)
app.MapGet("/users", () =>
{
  return Results.Ok(userStore.GetAllUsernames());
});

// TODO: Add endpoints for updating and deleting users

app.MapGet("/messages", () =>
{
  var messages = messageStore.GetAll();
  return Results.Ok(messages);
});

app.MapPost("/send-message", (MessageDTO dto) =>
{
  // Validate basic input
  if (string.IsNullOrWhiteSpace(dto.Content))
    return Results.BadRequest(new { Message = "Message content cannot be empty" });

  // Look up the user
  if (string.IsNullOrWhiteSpace(dto.Sender))
    return Results.BadRequest(new { Message = "Sender cannot be empty" });

  // Add message to store
  var added = messageStore.Add(dto.Sender, dto.Content);
  if (!added)
    return Results.BadRequest(new { Message = "Failed to add message" }); // unlikely with current store, but safe

  return Results.Ok(new { Message = "Message stored" });
});

app.Run();
