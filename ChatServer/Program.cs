using Shared;
using ChatServer.Store;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Create a single shared UserStore instance.
// The store contains both dictionaries (by username and by id).
UserStore userStore = new();

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

// TODO: add endpoints for sending and receiving chat messages

app.Run();
