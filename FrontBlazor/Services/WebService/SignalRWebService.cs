using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace FrontBlazor.Services;

public class SignalRWebService : IAsyncDisposable, ISignalRService
{
    private HubConnection? _chatHubConnection;
    private HubConnection? _notificationHubConnection;
    private readonly string _chatHubUrl;
    private readonly string _notificationHubUrl;

    // Événements existants pour le chat
    public event Action<int, int, bool>? OnProposalResponse;
    public event Action<int, int, string, List<int>, DateTime>? OnMessageReceived;
    public event Action<int, int, string>? OnUserTyping;
    public event Action<int, int>? OnMessagesRead;
    public event Action<int, int, int, decimal, DateTime>? OnPriceProposalReceived;

    // ✅ NOUVEAU : Événement pour les notifications
    public event Action<int>? OnNotificationCountUpdated;

    public bool IsConnected => _chatHubConnection?.State == HubConnectionState.Connected;
    public bool IsNotificationConnected => _notificationHubConnection?.State == HubConnectionState.Connected;

    public SignalRWebService()
    {
        _chatHubUrl = "http://localhost:5096/chatHub";
        _notificationHubUrl = "http://localhost:5096/notificationHub";
        Console.WriteLine($"[SignalR] Service initialized");
        Console.WriteLine($"[SignalR] Chat Hub URL: {_chatHubUrl}");
        Console.WriteLine($"[SignalR] Notification Hub URL: {_notificationHubUrl}");
    }

    public async Task StartAsync()
    {
        if (_chatHubConnection != null && IsConnected)
        {
            Console.WriteLine("[SignalR] Chat already connected — ignoring StartAsync()");
            return;
        }

        try
        {
            Console.WriteLine("[SignalR] Creating chat hub connection...");
            
            _chatHubConnection = new HubConnectionBuilder()
                .WithUrl(_chatHubUrl)
                .WithAutomaticReconnect(new[] 
                { 
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(2), 
                    TimeSpan.FromSeconds(5), 
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                })
                .Build();

            // Événements de connexion
            _chatHubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"[SignalR Chat] 🔄 Reconnecting... Error: {error?.Message}");
                return Task.CompletedTask;
            };

            _chatHubConnection.Reconnected += connectionId =>
            {
                Console.WriteLine($"[SignalR Chat] ✅ Reconnected! New connection ID: {connectionId}");
                return Task.CompletedTask;
            };

            _chatHubConnection.Closed += error =>
            {
                Console.WriteLine($"[SignalR Chat] ❌ Connection closed. Error: {error?.Message}");
                return Task.CompletedTask;
            };

            // Écoute des messages
            _chatHubConnection.On<int, int, string, List<int>, DateTime>(
                "ReceiveMessage",
                (conversationId, senderId, message, photos, date) =>
                {
                    Console.WriteLine($"[SignalR] 📨 ReceiveMessage event received:");
                    Console.WriteLine($"  - ConversationId: {conversationId}");
                    Console.WriteLine($"  - SenderId: {senderId}");
                    Console.WriteLine($"  - Message: {message}");
                    Console.WriteLine($"  - Photos: {string.Join(", ", photos)}");
                    Console.WriteLine($"  - Date: {date}");

                    OnMessageReceived?.Invoke(conversationId, senderId, message, photos, date);
                });

            _chatHubConnection.On<int, int, string>("UserTyping", (conversationId, userId, userName) =>
            {
                OnUserTyping?.Invoke(conversationId, userId, userName);
            });

            _chatHubConnection.On<int, int>("MessagesRead", 
                (conversationId, userId) =>
            {
                Console.WriteLine($"[SignalR] ✔️ MessagesRead: conv={conversationId}, user={userId}");
                OnMessagesRead?.Invoke(conversationId, userId);
            });
            
            _chatHubConnection.On<int, int, int, decimal, DateTime>(
                "ReceivePriceProposal",
                (conversationId, messageId, senderId, prixPropose, date) =>
                {
                    if (OnPriceProposalReceived != null)
                    {
                        OnPriceProposalReceived.Invoke(conversationId, messageId, senderId, prixPropose, date);
                        Console.WriteLine($"[SignalR]   ✅ Event invoked successfully");
                    }
                    else
                    {
                        Console.WriteLine($"[SignalR]   ⚠️ No subscribers for OnPriceProposalReceived!");
                    }
                    Console.WriteLine("========================================");
                });

            _chatHubConnection.On<int, int, bool>(
                "ProposalResponseReceived", 
                (conversationId, messageId, accepted) =>
                {
                    if (OnProposalResponse != null)
                    {
                        Console.WriteLine($"[SignalR]   Invoking OnProposalResponse event...");
                        OnProposalResponse.Invoke(conversationId, messageId, accepted);
                        Console.WriteLine($"[SignalR]   ✅ Event invoked successfully");
                    }
                    else
                    {
                        Console.WriteLine($"[SignalR]   ⚠️ No subscribers for OnProposalResponse!");
                    }
                    Console.WriteLine("========================================");
                });

            // Démarrage de la connexion chat
            Console.WriteLine("[SignalR] Starting chat connection...");
            await _chatHubConnection.StartAsync();
            Console.WriteLine("[SignalR Chat] ✅ Connection started successfully!");
            Console.WriteLine($"[SignalR Chat] Connection ID: {_chatHubConnection.ConnectionId}");
            Console.WriteLine($"[SignalR Chat] State: {_chatHubConnection.State}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR Chat] ❌ ERROR while starting connection:");
            Console.WriteLine($"  Message: {ex.Message}");
            Console.WriteLine($"  StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    // ✅ NOUVEAU : Démarrer le hub de notifications
    public async Task StartNotificationHubAsync()
{
    if (_notificationHubConnection != null && IsNotificationConnected)
    {
        Console.WriteLine("[SignalR] Notification hub already connected");
        return;
    }

    try
    {
        Console.WriteLine("[SignalR] Creating notification hub connection...");
        
        // ✅ Configuration SIMPLE pour Blazor WebAssembly
        _notificationHubConnection = new HubConnectionBuilder()
            .WithUrl(_notificationHubUrl)  // ✅ Pas de configuration supplémentaire !
            .WithAutomaticReconnect(new[] 
            { 
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2), 
                TimeSpan.FromSeconds(5), 
                TimeSpan.FromSeconds(10)
            })
            .Build();

        _notificationHubConnection.Reconnecting += error =>
        {
            Console.WriteLine($"[SignalR Notification] 🔄 Reconnecting... Error: {error?.Message}");
            return Task.CompletedTask;
        };

        _notificationHubConnection.Reconnected += connectionId =>
        {
            Console.WriteLine($"[SignalR Notification] ✅ Reconnected! Connection ID: {connectionId}");
            return Task.CompletedTask;
        };

        _notificationHubConnection.Closed += error =>
        {
            Console.WriteLine($"[SignalR Notification] ❌ Connection closed. Error: {error?.Message}");
            return Task.CompletedTask;
        };

        // Écoute de la mise à jour du compteur de notifications
        _notificationHubConnection.On<int>("UpdateNotificationCount", (count) =>
        {
            Console.WriteLine($"[SignalR Notification] 🔔 Received notification count: {count}");
            OnNotificationCountUpdated?.Invoke(count);
        });

        await _notificationHubConnection.StartAsync();
        Console.WriteLine("[SignalR Notification] ✅ Connection started successfully!");
        Console.WriteLine($"[SignalR Notification] Connection ID: {_notificationHubConnection.ConnectionId}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SignalR Notification] ❌ ERROR while starting connection:");
        Console.WriteLine($"  Message: {ex.Message}");
        Console.WriteLine($"  StackTrace: {ex.StackTrace}");
        _notificationHubConnection = null;
    }
}
    public async Task StopAsync()
    {
        // Arrêter le chat hub
        if (_chatHubConnection != null)
        {
            try
            {
                Console.WriteLine("[SignalR Chat] Stopping connection...");
                await _chatHubConnection.StopAsync();
                await _chatHubConnection.DisposeAsync();
                Console.WriteLine("[SignalR Chat] Connection stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR Chat] Error stopping connection: {ex.Message}");
            }
            _chatHubConnection = null;
        }

        // Arrêter le notification hub
        await StopNotificationHubAsync();
    }

    // ✅ NOUVEAU : Arrêter le hub de notifications
    public async Task StopNotificationHubAsync()
    {
        if (_notificationHubConnection != null)
        {
            try
            {
                Console.WriteLine("[SignalR Notification] Stopping connection...");
                await _notificationHubConnection.StopAsync();
                await _notificationHubConnection.DisposeAsync();
                Console.WriteLine("[SignalR Notification] Connection stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR Notification] Error stopping: {ex.Message}");
            }
            _notificationHubConnection = null;
        }
    }

    // Méthodes existantes pour le chat
    public async Task JoinConversation(int conversationId)
    {
        if (_chatHubConnection == null || !IsConnected)
        {
           return;
        }

        try
        {
             await _chatHubConnection.InvokeAsync("JoinConversation", conversationId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error joining conversation {conversationId}:");
            Console.WriteLine($"[SignalR]    Message: {ex.Message}");
            throw;
        }
    }

    public async Task LeaveConversation(int conversationId)
    {
        if (_chatHubConnection == null || !IsConnected)
        {
            Console.WriteLine($"[SignalR] ⚠️ Cannot leave conversation {conversationId}: not connected");
            return;
        }

        try
        {
            Console.WriteLine($"[SignalR] Leaving conversation {conversationId}...");
            await _chatHubConnection.InvokeAsync("LeaveConversation", conversationId);
            Console.WriteLine($"[SignalR] ✅ Successfully left conversation {conversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error leaving conversation {conversationId}: {ex.Message}");
        }
    }

    public async Task SendMessage(int conversationId, int senderId, string message, List<int> photoIds)
    {
        if (_chatHubConnection == null || !IsConnected)
            throw new InvalidOperationException("SignalR connection is not established");

        Console.WriteLine($"[SignalR] Sending message:");
        Console.WriteLine($"  - ConversationId: {conversationId}");
        Console.WriteLine($"  - SenderId: {senderId}");
        Console.WriteLine($"  - Message: {message}");

        await _chatHubConnection.InvokeAsync("SendMessage", conversationId, senderId, message, photoIds);
        Console.WriteLine($"[SignalR] ✅ Message sent successfully");
    }

    public async Task NotifyTyping(int conversationId, int userId, string userName)
    {
        if (_chatHubConnection == null || !IsConnected)
        {
            Console.WriteLine($"[SignalR] ⚠️ Cannot notify typing: not connected");
            return;
        }

        try
        {
            await _chatHubConnection.InvokeAsync("NotifyTyping", conversationId, userId, userName);
            Console.WriteLine($"[SignalR] ⌨️ Typing notification sent for conversation {conversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error notifying typing: {ex.Message}");
        }
    }

    public async Task MarkMessagesAsRead(int conversationId, int userId)
    {
        Console.WriteLine($"[SignalR] 🔍 MarkMessagesAsRead called:");
        Console.WriteLine($"  - ConversationId: {conversationId}");
        Console.WriteLine($"  - UserId: {userId}");
        Console.WriteLine($"  - Connection State: {_chatHubConnection?.State}");
        Console.WriteLine($"  - Connection ID: {_chatHubConnection?.ConnectionId}");
    
        if (_chatHubConnection?.State != HubConnectionState.Connected)
        {
            Console.WriteLine($"[SignalR] ⚠️ Cannot mark as read - not connected!");
            return;
        }

        try
        {
            Console.WriteLine($"[SignalR] 🚀 Invoking MarkMessagesAsRead on hub...");
            await _chatHubConnection.InvokeAsync("MarkMessagesAsRead", conversationId, userId);
            Console.WriteLine($"[SignalR] ✅ Successfully invoked MarkMessagesAsRead");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error marking messages as read:");
            Console.WriteLine($"  - Error Type: {ex.GetType().Name}");
            Console.WriteLine($"  - Message: {ex.Message}");
        
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  - Inner Exception: {ex.InnerException.Message}");
            }
        }
    }

    public async Task NotifyProposalResponse(int conversationId, int messageId, bool accepted)
    {
        if (_chatHubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _chatHubConnection.InvokeAsync(
                    "NotifyProposalResponse", 
                    conversationId, 
                    messageId, 
                    accepted
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR] ❌ Error notifying proposal response: {ex.Message}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("[SignalR] DisposeAsync called");
        await StopAsync();
    }
}