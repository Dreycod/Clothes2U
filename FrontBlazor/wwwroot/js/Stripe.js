// Placez ce fichier dans wwwroot/js/stripe.js

let stripe;
let elements;
let paymentElement;

// Initialiser Stripe avec votre clé publique
window.initializeStripe = function(publishableKey) {
    if (!stripe) {
        stripe = Stripe(publishableKey);
        console.log('[Stripe] Initialized with publishable key');
    }
};

// Initialiser Stripe Elements avec le client secret
window.initializeStripeElements = async function(clientSecret) {
    try {
        console.log('[Stripe] Initializing Elements with clientSecret:', clientSecret.substring(0, 20) + '...');

        if (!stripe) {
            console.error('[Stripe] Stripe not initialized! Call initializeStripe first.');
            return;
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

        paymentElement.mount('#payment-element');

        console.log('[Stripe] ✅ Payment Element mounted successfully');

        // Écouter les événements du formulaire
        paymentElement.on('ready', () => {
            console.log('[Stripe] Payment Element is ready');
        });

        paymentElement.on('change', (event) => {
            if (event.error) {
                console.error('[Stripe] Payment Element error:', event.error.message);
            } else {
                console.log('[Stripe] Payment Element changed:', event.complete ? 'Complete' : 'Incomplete');
            }
        });

    } catch (error) {
        console.error('[Stripe] ❌ Error initializing Elements:', error);
        throw error;
    }
};

// Confirmer le paiement
window.confirmStripePayment = async function() {
    try {
        console.log('[Stripe] Confirming payment...');

        if (!stripe || !elements) {
            throw new Error('Stripe not properly initialized');
        }

        // Soumettre le formulaire pour valider
        const {error: submitError} = await elements.submit();
        if (submitError) {
            console.error('[Stripe] ❌ Submit error:', submitError.message);
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
            console.error('[Stripe] ❌ Payment error:', error.message);
            return {
                success: false,
                errorMessage: error.message
            };
        }

        console.log('[Stripe] ✅ Payment confirmed:', paymentIntent.id);
        console.log('[Stripe]   Status:', paymentIntent.status);

        if (paymentIntent.status === 'succeeded') {
            return {
                success: true,
                paymentIntentId: paymentIntent.id
            };
        } else if (paymentIntent.status === 'requires_action') {
            // Authentification 3D Secure en cours
            console.log('[Stripe] ⚠️ Requires additional action (3D Secure)');
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