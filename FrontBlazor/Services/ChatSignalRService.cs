using FrontBlazor.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace FrontBlazor.Services;

public class ChatSignalRService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    public event Action<int, Message>? OnMessageReceived;

    public async Task StartAsync(string hubUrl)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<int, Message>("ReceiveMessage", (conversationId, message) =>
        {
            OnMessageReceived?.Invoke(conversationId, message);
        });

        await _hubConnection.StartAsync();
        Console.WriteLine("SignalR connection started");
    }

    public async Task JoinConversation(int conversationId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("JoinConversation", conversationId);
            Console.WriteLine($"Joined conversation {conversationId}");
        }
    }

    public async Task LeaveConversation(int conversationId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("LeaveConversation", conversationId);
        }
    }

    public async Task SendMessage(int conversationId, string message, int userId)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            await _hubConnection.SendAsync("SendMessageToConversation", conversationId, message, userId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}