using DistChat.Node.Auth.Application;
using DistChat.Node.Auth.Database;
using DistChat.Node.Auth.Services;
using DistChat.Node.Functionality.Application.Chat;
using DistChat.Node.Functionality.Application.Users;
using DistChat.Node.Functionality.Database.Chat;
using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Infrastructure.RealtimeHub;
using MyApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Infrastructure
builder.Services.AddScoped<EmailService, EmailService>();

// Functionality - database
builder.Services.AddScoped<IRoomDbService, RoomDbService>();
builder.Services.AddScoped<IMessageDbService, MessageDbService>();
builder.Services.AddScoped<IFriendshipDbService, FriendshipDbService>();
builder.Services.AddScoped<IUserDbService, UserDbService>();

// Functionaliry - application
builder.Services.AddScoped<UserConnectionTracker>();
builder.Services.AddScoped<RoomFocusTracker>();

// Functionality - application/chat
builder.Services.AddScoped<MessageReceived>();
builder.Services.AddScoped<RemovedFromRoom>();
builder.Services.AddScoped<AddedToRoom>();
builder.Services.AddScoped<UserJoined>();
builder.Services.AddScoped<UserLeft>();
builder.Services.AddScoped<ChatSynchronization>();

builder.Services.AddScoped<IChatOperations, ChatOperations>();
builder.Services.AddScoped<ChatRealtimeHandler>();

// Functionaliry - application/friendship
builder.Services.AddScoped<FriendshipRequested>();
builder.Services.AddScoped<FriendshipAccepted>();
builder.Services.AddScoped<FriendshipRejected>();
builder.Services.AddScoped<FriendRemoved>();

builder.Services.AddScoped<IFriendshipOperations, FriendshipOperations>();
builder.Services.AddScoped<FriendshipRealtimeHandler>();

// Auth - database
builder.Services.AddScoped<IAuthDbService, AuthDbService>();
builder.Services.AddScoped<IRegistrationDbService, RegistrationDbService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenCookieHandler, TokenCookieHandler>();

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();


