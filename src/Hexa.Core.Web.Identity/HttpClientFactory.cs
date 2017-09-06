namespace Hexa.Core.Web.Identity
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public interface IHttpClientFactory
    {
        Task<HttpClient> Create(Uri uri, string requestScope);
    }

    public class HttpClientFactory : IHttpClientFactory
    {
        public const string EndUserHeaderName = "pos-end-user";
        private readonly TokenClient tokenClient;
        private readonly string correlationId;
        private readonly string idToken;

        public HttpClientFactory(string tokenUrl, string clientName, string clientSecret, string correlationId, string idToken)
        {
            this.tokenClient = new TokenClient(tokenUrl, clientName, clientSecret);
            this.correlationId = correlationId;
            this.idToken = idToken;
        }

        public async Task<HttpClient> Create(Uri uri, string requestScope)
        {
            var response = await this.tokenClient.RequestClientCredentialsAsync(requestScope).ConfigureAwait(false);
            var client = new HttpClient() { BaseAddress = uri };
            client.SetBearerToken(response.AccessToken);

            client.DefaultRequestHeaders.Add(HttpClientFactoryHelper.CorrelationIdHeaderKey, this.correlationId);

            if (!string.IsNullOrEmpty(this.idToken))
            {
                client.DefaultRequestHeaders.Add(EndUserHeaderName, this.idToken);
            }

            return client;
        }
    }
}