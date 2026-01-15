// Placez ce fichier dans wwwroot/js/stripe.js

let stripe;
let elements;
let paymentElement;

// Initialiser Stripe avec votre clé publique
window.initializeStripe = function(publishableKey) {

    // ✅ FIX: Check if already initialized and return success
    if (stripe) {
        return true;
    }

    if (!publishableKey || !publishableKey.startsWith('pk_')) {
        return false;
    }

    try {
        stripe = Stripe(publishableKey);
        return true;
    } catch (error) {
        return false;
    }
};

// Initialiser Stripe Elements avec le client secret
window.initializeStripeElements = async function(clientSecret) {
    try {
        
        if (!stripe) {
            return false; // ✅ FIX: Return false instead of throwing
        }

        // Nettoyer les éléments existants
        if (paymentElement) {
            paymentElement.unmount();
            paymentElement = null;
        }

        // Créer une instance d'Elements
        elements = stripe.elements({
            clientSecret: clientSecret,
            appearance: {
                theme: 'stripe',
                variables: {
                    colorPrimary: '#7a9d8e',
                    colorBackground: '#ffffff',
                    colorText: '#1a1a1a',
                    colorDanger: '#ef4444',
                    fontFamily: 'system-ui, -apple-system, sans-serif',
                    spacingUnit: '4px',
                    borderRadius: '8px',
                }
            }
        });

        // Créer et monter le Payment Element
        paymentElement = elements.create('payment', {
            layout: {
                type: 'accordion',
                defaultCollapsed: false,
                radios: true,
                spacedAccordionItems: true
            }
        });

        // Vérifier que le conteneur existe
        const container = document.getElementById('payment-element');
        if (!container) {
           return false; // ✅ FIX: Return false instead of throwing
        }

        paymentElement.mount('#payment-element');

        
        // Écouter les événements du formulaire
        paymentElement.on('ready', () => {
            console.log('[Stripe] 📝 Payment Element is ready');
        });

        paymentElement.on('change', (event) => {
            if (event.error) {
                console.error('[Stripe] ⚠️ Payment Element error:', event.error.message);
            } else {
                console.log('[Stripe] ✏️ Payment Element changed:', event.complete ? 'Complete' : 'Incomplete');
            }
        });

        return true;

    } catch (error) {
        return false;
    }
};

// Confirmer le paiement
window.confirmStripePayment = async function() {
    try {
        if (!stripe || !elements) {
            return {
                success: false,
                errorMessage: 'Stripe not properly initialized. Please refresh the page and try again.'
            };
        }

        // Soumettre le formulaire pour valider
        const {error: submitError} = await elements.submit();
        if (submitError) {
            return {
                success: false,
                errorMessage: submitError.message
            };
        }

        // Confirmer le paiement
        const {error, paymentIntent} = await stripe.confirmPayment({
            elements,
            redirect: 'if_required',
            confirmParams: {
                return_url: window.location.origin + '/purchase/success'
            }
        });

        if (error) {
            return {
                success: false,
                errorMessage: error.message
            };
        }

        if (paymentIntent.status === 'succeeded') {
            return {
                success: true,
                paymentIntentId: paymentIntent.id
            };
        } else if (paymentIntent.status === 'requires_action') {
            // Authentification 3D Secure en cours
            return {
                success: false,
                errorMessage: 'Authentication required. Please try again.'
            };
        } else {
            return {
                success: false,
                errorMessage: `Payment status: ${paymentIntent.status}`
            };
        }

    } catch (error) {
        console.error('[Stripe] ❌ Unexpected error:', error);
        return {
            success: false,
            errorMessage: error.message || 'An unexpected error occurred'
        };
    }
};

// Cleanup (optionnel)
window.destroyStripeElements = function() {
    if (paymentElement) {
        paymentElement.destroy();
        console.log('[Stripe] Payment Element destroyed');
    }
    elements = null;
    paymentElement = null;
};