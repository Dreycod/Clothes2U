using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace FrontBlazor.Services;

public class SignalRWebService : IAsyncDisposable, ISignalRService
{
    private HubConnection? _hubConnection;
    private readonly string _hubUrl;
    public event Action<int, int, bool>? OnProposalResponse;

    public event Action<int, int, string, List<int>, DateTime>? OnMessageReceived;
    public event Action<int, int, string>? OnUserTyping;
    public event Action<int, int>? OnMessagesRead;
    public event Action<int, int, int, decimal, DateTime>? OnPriceProposalReceived;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

    public SignalRWebService()
    {
        _hubUrl = "http://localhost:5096/chatHub"; // ← Vérifiez que l'URL est correcte
        Console.WriteLine($"[SignalR] Service initialized with hub URL: {_hubUrl}");
    }

    public async Task StartAsync()
    {
        if (_hubConnection != null && IsConnected)
        {
            Console.WriteLine("[SignalR] Already connected — ignoring StartAsync()");
            return;
        }

        try
        {
            Console.WriteLine("[SignalR] Creating hub connection...");
            
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    // Activer les credentials si nécessaire pour l'authentification
                    options.AccessTokenProvider = async () =>
                    {
                        // Si vous utilisez des JWT, récupérez le token ici
                        // var token = await GetTokenAsync();
                        // return token;
                        return null;
                    };
                })
                .WithAutomaticReconnect(new[] 
                { 
                    TimeSpan.Zero,           // Reconnexion immédiate
                    TimeSpan.FromSeconds(2), 
                    TimeSpan.FromSeconds(5), 
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .ConfigureLogging(logging =>
                {
                    // Active les logs SignalR dans la console
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
                })
                .Build();

            // ===== ÉVÉNEMENTS DE CONNEXION =====
            
            _hubConnection.Reconnecting += error =>
            {
                Console.WriteLine($"[SignalR] 🔄 Reconnecting... Error: {error?.Message}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                Console.WriteLine($"[SignalR] ✅ Reconnected! New connection ID: {connectionId}");
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                Console.WriteLine($"[SignalR] ❌ Connection closed. Error: {error?.Message}");
                return Task.CompletedTask;
            };

            // ===== ÉCOUTE DES MESSAGES DU SERVEUR =====
            
            // Réception d'un nouveau message
            _hubConnection.On<int, int, string, List<int>, DateTime>(
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

            // Notification qu'un utilisateur est en train d'écrire
            _hubConnection.On<int, int, string>("UserTyping", (conversationId, userId, userName) =>
            {
                OnUserTyping?.Invoke(conversationId, userId, userName);
            });

            // Notification que des messages ont été lus
            _hubConnection.On<int, int>("MessagesRead", 
                (conversationId, userId) =>
            {
                Console.WriteLine($"[SignalR] ✔️ MessagesRead: conv={conversationId}, user={userId}");
                OnMessagesRead?.Invoke(conversationId, userId);
            });
            
            _hubConnection.On<int, int, int, decimal, DateTime>(
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

            // 🆕 Réception d'une réponse à une proposition
            _hubConnection.On<int, int, bool>(
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

            // ===== DÉMARRAGE DE LA CONNEXION =====
            
            Console.WriteLine("[SignalR] Starting connection...");
            await _hubConnection.StartAsync();
            Console.WriteLine("[SignalR] ✅ Connection started successfully!");
            Console.WriteLine($"[SignalR] Connection ID: {_hubConnection.ConnectionId}");
            Console.WriteLine($"[SignalR] State: {_hubConnection.State}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ ERROR while starting connection:");
            Console.WriteLine($"  Message: {ex.Message}");
            Console.WriteLine($"  StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task StopAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                Console.WriteLine("[SignalR] Stopping connection...");
                await _hubConnection.StopAsync();
                Console.WriteLine("[SignalR] Connection stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR] Error stopping connection: {ex.Message}");
            }

            try
            {
                await _hubConnection.DisposeAsync();
                Console.WriteLine("[SignalR] HubConnection disposed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SignalR] Error disposing: {ex.Message}");
            }

            _hubConnection = null;
        }
    }

    public async Task JoinConversation(int conversationId)
    {
        if (_hubConnection == null || !IsConnected)
        {
           return;
        }

        try
        {
             await _hubConnection.InvokeAsync("JoinConversation", conversationId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error joining conversation {conversationId}:");
            Console.WriteLine($"[SignalR]    Message: {ex.Message}");
            Console.WriteLine($"[SignalR]    StackTrace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task LeaveConversation(int conversationId)
    {
        if (_hubConnection == null || !IsConnected)
        {
            Console.WriteLine($"[SignalR] ⚠️ Cannot leave conversation {conversationId}: not connected");
            return;
        }

        try
        {
            Console.WriteLine($"[SignalR] Leaving conversation {conversationId}...");
            await _hubConnection.InvokeAsync("LeaveConversation", conversationId);
            Console.WriteLine($"[SignalR] ✅ Successfully left conversation {conversationId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error leaving conversation {conversationId}: {ex.Message}");
        }
    }

    public async Task SendMessage(int conversationId, int senderId, string message, List<int> photoIds)
    {
        if (_hubConnection == null || !IsConnected)
            throw new InvalidOperationException("SignalR connection is not established");

        Console.WriteLine($"[SignalR] Sending message:");
        Console.WriteLine($"  - ConversationId: {conversationId}");
        Console.WriteLine($"  - SenderId: {senderId}");
        Console.WriteLine($"  - Message: {message}");
        Console.WriteLine($"  - Photos: {string.Join(", ", photoIds)}");

        await _hubConnection.InvokeAsync("SendMessage", conversationId, senderId, message, photoIds);
        Console.WriteLine($"[SignalR] ✅ Message sent successfully");
    }

    public async Task NotifyTyping(int conversationId, int userId, string userName)
    {
        if (_hubConnection == null || !IsConnected)
        {
            Console.WriteLine($"[SignalR] ⚠️ Cannot notify typing: not connected");
            return;
        }

        try
        {
            // ✅ CORRECTION : Le Hub a une méthode "NotifyTyping"
            await _hubConnection.InvokeAsync("NotifyTyping", conversationId, userId, userName);
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
        Console.WriteLine($"  - Connection State: {_hubConnection?.State}");
        Console.WriteLine($"  - Connection ID: {_hubConnection?.ConnectionId}");
    
        if (_hubConnection?.State != HubConnectionState.Connected)
        {
            Console.WriteLine($"[SignalR] ⚠️ Cannot mark as read - not connected!");
            return;
        }

        try
        {
            Console.WriteLine($"[SignalR] 🚀 Invoking MarkMessagesAsRead on hub...");
            await _hubConnection.InvokeAsync("MarkMessagesAsRead", conversationId, userId);
            Console.WriteLine($"[SignalR] ✅ Successfully invoked MarkMessagesAsRead");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SignalR] ❌ Error marking messages as read:");
            Console.WriteLine($"  - Error Type: {ex.GetType().Name}");
            Console.WriteLine($"  - Message: {ex.Message}");
            Console.WriteLine($"  - Stack: {ex.StackTrace}");
        
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  - Inner Exception: {ex.InnerException.Message}");
            }
        }

    }


    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("[SignalR] DisposeAsync called");
        await StopAsync();
    }
    public async Task NotifyProposalResponse(int conversationId, int messageId, bool accepted)
    {
        if (_hubConnection?.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.InvokeAsync(
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
    
    private void SetupHandlers()
    {
        _hubConnection.On<int, int, bool>("ProposalResponseReceived", 
            (conversationId, messageId, accepted) =>
            {
                Console.WriteLine($"[SignalR] 📨 Proposal response received: Conv={conversationId}, Msg={messageId}, Accepted={accepted}");
                OnProposalResponse?.Invoke(conversationId, messageId, accepted);
            });
    }
}
