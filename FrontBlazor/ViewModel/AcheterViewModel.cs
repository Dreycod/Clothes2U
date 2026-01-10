using System.Collections.ObjectModel;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTO;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Shared;
using Shared.DTO.Annonce;
using Shared.DTO.Conversation;
using Shared.DTO.Message;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel;

public class AcheterViewModel : ClientBaseViewModel, IDisposable
{
    private readonly IAuthService _authService;
    private readonly IPaymentService _paymentService;
    private readonly IAnnonceService _annonceService;
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly IMessageService _messageService;
    private readonly IOrderService _orderService;
    private readonly NavigationManager _nav;
    private readonly IJSRuntime _jsRuntime;

    public ConversationDTO? SelectedConversation { get; private set; }
    public AnnonceDetailDTO? SelectedAnnonce { get; private set; }
    public AdresseDTO? SelectedAddress { get; private set; }
    public List<AdresseDTO> UserAddresses { get; private set; } = new();
    
    // États du processus
    public bool IsLoading { get; private set; }
    public bool IsProcessingPayment { get; private set; }
    public string? ErrorMessage { get; private set; }
    public PurchaseStep CurrentStep { get; private set; } = PurchaseStep.AddressSelection;
    
    // Données de paiement
    public string? StripeClientSecret { get; private set; }
    public string? PaymentIntentId { get; private set; }
    
    // Prix et frais
    public decimal ProductPrice => SelectedConversation!.Prix;
    public decimal ServiceFee => ProductPrice * (decimal)0.05; // 5% de frais de service
    public decimal ShippingCost => (decimal)5.99; // Frais de livraison fixe
    public decimal TotalAmount => ProductPrice + ServiceFee + ShippingCost;

    public event Action? OnChange;

    public AcheterViewModel(
        IAuthService authService,
        IPaymentService paymentService,
        IAnnonceService annonceService,
        IConversationService<ConversationDTO> conversationService,
        IOrderService orderService,
        IMessageService messageService,
        NavigationManager nav,
        NavigationManager navigationManager,
        INotificationService notificationService,
        IJSRuntime jsRuntime): base(navigationManager, authService, notificationService)
    {
        _authService = authService;
        _paymentService = paymentService;
        _annonceService = annonceService;
        _messageService = messageService;
        _conversationService = conversationService;
        _orderService = orderService;
        _nav = nav;
        _jsRuntime = jsRuntime;
    }

    public async Task LoadAsync(int conversationId)
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();
        await base.LoadAsync();

        try
        {
            if (utilisateur == null)
            {
                _nav.NavigateTo("/login");
                return;
            }
            // Charger l'annonce
            SelectedConversation = await _conversationService.GetConversationDetailById(conversationId);
            if (SelectedConversation == null)
            {
                ErrorMessage = "Conversation introuvable";
                return;
            }
            SelectedAnnonce = await _annonceService.GetAnnonceDetailById(SelectedConversation!.AnnonceId.Value);
            if (SelectedAnnonce == null)
            {
                ErrorMessage = "Annonce introuvable";
                return;
            }
            Console.WriteLine("3");
            // Vérifier que l'annonce est disponible
            if (SelectedAnnonce.StatutAnnonce != "En Ligne")
            {
                ErrorMessage = "Cette annonce n'est plus disponible";
                return;
            }
            Console.WriteLine("4");
            // Vérifier que l'utilisateur n'achète pas son propre article
            if (SelectedAnnonce.UtilisateurId == utilisateur.UtilisateurId)
            {
                ErrorMessage = "Vous ne pouvez pas acheter votre propre article";
                return;
            }
            Console.WriteLine("5");
            // Charger les adresses de l'utilisateur
            await LoadUserAddresses();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PurchaseVM] ❌ Error loading: {ex.Message}");
            ErrorMessage = "Une erreur est survenue lors du chargement";
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    private async Task LoadUserAddresses()
    {
        try
        {
            var addresses = await _authService.GetUserAddressesAsync();
            UserAddresses = addresses != null ? addresses : new List<AdresseDTO>();
            
            // Sélectionner l'adresse par défaut si elle existe
            SelectedAddress = UserAddresses.FirstOrDefault() ?? new AdresseDTO();
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PurchaseVM] ❌ Error loading addresses: {ex.Message}");
        }
    }

    public void SelectAddress(AdresseDTO address)
    {
        SelectedAddress = address;
        NotifyStateChanged();
    }

    public async Task ProceedToPayment()
    {
        if (SelectedAddress == null)
        {
            ErrorMessage = "Veuillez sélectionner une adresse de livraison";
            NotifyStateChanged();
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            // Créer un PaymentIntent sur Stripe
            var paymentIntent = await _paymentService.CreatePaymentIntentAsync(new CreatePaymentIntentDTO
            {
                Amount = (int)(TotalAmount * 100), // Stripe utilise les centimes
                Currency = "eur",
                AnnonceId = SelectedAnnonce!.AnnonceId,
                UserId = utilisateur!.UtilisateurId,
                AddressId = SelectedAddress.AdresseId
            });

            StripeClientSecret = paymentIntent.ClientSecret;
            PaymentIntentId = paymentIntent.PaymentIntentId;
            
            CurrentStep = PurchaseStep.Payment;
            
            // Initialiser Stripe Elements dans la page
            await _jsRuntime.InvokeVoidAsync("initializeStripeElements", StripeClientSecret);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PurchaseVM] ❌ Error creating payment intent: {ex.Message}");
            ErrorMessage = "Impossible d'initialiser le paiement. Veuillez réessayer.";
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }
    

    public async Task<bool> ConfirmPayment()
    {
        IsProcessingPayment = true;
        ErrorMessage = null;
        NotifyStateChanged();

        try
        {
            // Confirmer le paiement via Stripe (géré par JavaScript)
            var result = await _jsRuntime.InvokeAsync<PaymentResult>("confirmStripePayment");

            if (result.Success)
            {
                // Créer la commande dans la base de données
                await CreateOrder(result.PaymentIntentId);
                
                CurrentStep = PurchaseStep.Success;

                MessageEstPayeePostDTO messagePayeeDTO = new MessageEstPayeePostDTO
                {
                    ConversationId = SelectedConversation.ConversationId,
                    UtilisateurId = utilisateur.UtilisateurId,
                    
                };

                _messageService.PostMessagePayee(messagePayeeDTO);
                
                return true;
            }
            else
            {
                ErrorMessage = result.ErrorMessage ?? "Le paiement a échoué";
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PurchaseVM] ❌ Error confirming payment: {ex.Message}");
            ErrorMessage = "Une erreur est survenue lors du paiement";
            return false;
        }
        finally
        {
            IsProcessingPayment = false;
            NotifyStateChanged();
        }
    }

    private async Task CreateOrder(string paymentIntentId)
    {
        var order = new CreateOrderDTO
        {
            AnnonceId = SelectedAnnonce!.AnnonceId,
            AcheteurId = utilisateur!.UtilisateurId,
            VendeurId = SelectedAnnonce.UtilisateurId,
            AdresseLivraisonId = SelectedAddress!.AdresseId,
            MontantTotal = TotalAmount,
            FraisService = ServiceFee,
            FraisLivraison = ShippingCost,
            StripePaymentIntentId = paymentIntentId,
            StatutCommande = "Payée"
        };
        
        Console.WriteLine("bchjdsfgcjherfgcjkhgercgy");
        
        await _orderService.CreateOrderAsync(order);

        SelectedAnnonce.EtatArticle = "Vendu";
        
        // Mettre à jour le statut de l'annonce
        await _annonceService.VendreAnnonce(SelectedAnnonce.AnnonceId);
    }

    public void GoBack()
    {
        if (CurrentStep == PurchaseStep.Payment)
        {
            CurrentStep = PurchaseStep.AddressSelection;
        }
        else
        {
            _nav.NavigateTo($"/product/{SelectedAnnonce?.AnnonceId}");
        }
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        // Cleanup si nécessaire
    }
}

public enum PurchaseStep
{
    AddressSelection,
    Payment,
    Success
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string? PaymentIntentId { get; set; }
    public string? ErrorMessage { get; set; }
}

// DTOs
