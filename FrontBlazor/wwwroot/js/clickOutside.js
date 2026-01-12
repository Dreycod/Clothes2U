window.setupClickOutside = (dotnetHelper, notificationElement, profileElement) => {
    const clickHandler = (event) => {
        const notificationContainer = document.querySelector('.notification-dropdown-container');
        const profileContainer = document.querySelector('.profile-dropdown-container');

        if (notificationContainer && !notificationContainer.contains(event.target) &&
            profileContainer && !profileContainer.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('CloseDropdowns');
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

window.setupProductActionsClickOutside = (dotnetHelper) => {
    const clickHandler = (event) => {
        const dropdownContainer = document.querySelector('.product-actions-dropdown-container');

        if (dropdownContainer && !dropdownContainer.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('CloseProductActionsDropdown');
        }
    };

    setTimeout(() => {
        document.addEventListener('click', clickHandler);
    }, 100);

    window.productActionsClickHandler = clickHandler;
};

window.removeProductActionsClickOutside = () => {
    if (window.productActionsClickHandler) {
        document.removeEventListener('click', window.productActionsClickHandler);
        window.productActionsClickHandler = null;
    }
};

window.setupProfileActionsClickOutside = (dotnetHelper) => {
    const clickHandler = (event) => {
        const dropdownContainer = document.querySelector('.profile-actions-dropdown-container');

        if (dropdownContainer && !dropdownContainer.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('CloseProfileActionsDropdown');
        }
    };

    setTimeout(() => {
        document.addEventListener('click', clickHandler);
    }, 100);

    window.profileActionsClickHandler = clickHandler;
};

window.removeProfileActionsClickOutside = () => {
    if (window.profileActionsClickHandler) {
        document.removeEventListener('click', window.profileActionsClickHandler);
        window.profileActionsClickHandler = null;
    }
};