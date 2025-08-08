namespace GetInitialFunctions.Services
{
    public interface ITagDiscoveryApiService
    {
        Task<string> GetInitialsSvgAsync(string name, CancellationToken cancellationToken);
    }

    public class TagDiscoveryApiService : ITagDiscoveryApiService
    {
        private readonly HttpClient _httpClient;

        public TagDiscoveryApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("tagdiscovery");
        }

        public async Task<string> GetInitialsSvgAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            }

            var response = await _httpClient.GetAsync($"/api/get-initials?name={name}", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error fetching initials SVG {response.ReasonPhrase}");
            }

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
