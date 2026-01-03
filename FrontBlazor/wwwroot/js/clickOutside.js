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