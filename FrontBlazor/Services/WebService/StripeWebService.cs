using FrontBlazor.ViewModel;
using Microsoft.JSInterop;

namespace FrontBlazor.Services;

public class StripeWebService
{
    
    private readonly IJSRuntime _js;
    private bool _stripeInitialized;
    private bool _elementsInitialized;

    public StripeWebService(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>
    /// Initialise Stripe avec la clé publique (UNE SEULE FOIS)
    /// </summary>
    public async Task InitializeStripeAsync(string publishableKey)
    {
        if (_stripeInitialized)
            return;

        await _js.InvokeVoidAsync("initializeStripe", publishableKey);
        _stripeInitialized = true;
    }

    /// <summary>
    /// Initialise Stripe Elements quand le DOM est prêt
    /// </summary>
    public async Task InitializeElementsAsync(string clientSecret)
    {
        if (!_stripeInitialized)
            throw new InvalidOperationException("Stripe must be initialized before Elements.");

        if (_elementsInitialized)
            return;

        await _js.InvokeVoidAsync("initializeStripeElements", clientSecret);
        _elementsInitialized = true;
    }

    /// <summary>
    /// Confirme le paiement Stripe
    /// </summary>
    public async Task<PaymentResult> ConfirmPaymentAsync()
    {
        if (!_stripeInitialized || !_elementsInitialized)
            throw new InvalidOperationException("Stripe is not fully initialized.");

        return await _js.InvokeAsync<PaymentResult>("confirmStripePayment");
    }

    /// <summary>
    /// Reset (utile si l'utilisateur revient en arrière)
    /// </summary>
    public void Reset()
    {
        _elementsInitialized = false;
    }
}