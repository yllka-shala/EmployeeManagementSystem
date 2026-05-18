using EMSBaseLibrary.DTOs;

namespace EMS_Client.Helpers
{
    public class GetHttpClient(IHttpClientFactory httpClientFactory, SessionService sessionService)
    {
        private const string HeaderKey = "Authorization";

        public async Task<HttpClient> GetPrivateHttpClient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");

            var stringToken = await sessionService.GetToken();
            if (string.IsNullOrEmpty(stringToken))
                return client;

            var deserializeToken = JsonHelper.DeserializeJsonString<UserSession>(stringToken);
            if (deserializeToken == null)
                return client;

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializeToken.Token);
            return client;
        }

        public HttpClient GetPublicHttpClient()
        {
            var client = httpClientFactory.CreateClient("SystemApiClient");
            client.DefaultRequestHeaders.Remove(HeaderKey);
            return client;
        }
    }
}
