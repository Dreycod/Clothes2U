window.setupClickOutside = (dotnetHelper, notificationElement, profileElement, profileActionsContainer) => {
    const clickHandler = (event) => {
        const notificationContainer = document.querySelector('.notification-dropdown-container');
        const profileContainer = document.querySelector('.profile-dropdown-container');
        const profileActionsContainer = document.querySelector('.profile-actions-dropdown-container');

        if (notificationContainer && !notificationContainer.contains(event.target) &&
            profileContainer && !profileContainer.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('CloseDropdowns');
        }

        if (profileActionsContainer && !profileActionsContainer.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('ClopseDropdowns');
        }
    };

    setTimeout(() => {
        document.addEventListener('click', clickHandler);
    }, 100);

    window.clickOutsideHandler = clickHandler;
};

window.removeClickOutside = () => {
    if (window.clickOutsideHandler) {
        document.removeEventListener('click', window.clickOutsideHandler);
        window.clickOutsideHandler = null;
    }
};