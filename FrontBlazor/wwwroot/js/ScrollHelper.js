window.ScrollHelper = (element) => {
    console.log("ScrollHelper.js LOADED");
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};