using Microsoft.JSInterop;

namespace FrontBlazor.Services
{
    public class ClipboardService
    {
        private readonly IJSRuntime _js;
        public ClipboardService(IJSRuntime js)
        {
            _js = js;
        }
        public void Copy(string text)
            => _js.InvokeVoidAsync("clipboard.copyText", text);
    }
}
