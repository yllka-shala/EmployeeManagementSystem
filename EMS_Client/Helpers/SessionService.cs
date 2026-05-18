using Microsoft.JSInterop;

namespace EMS_Client.Helpers
{
    public class SessionService(IJSRuntime js)
    {
        public const string StorageKey = "authentication-token";

        public async Task<string> GetToken() =>
            await js.InvokeAsync<string>("localStorage.getItem", StorageKey);

        public async Task SetToken(string item) =>
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, item);

        public async Task RemoveToken() =>
            await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
