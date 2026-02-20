using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;

namespace BlazorCloudflarePages.Function;

public class Joke
{
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;

    public Joke(ILogger<Joke> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    [Function("joke")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        _logger.LogInformation("Fetching Chuck Norris joke");

        try
        {
            var response = await _httpClient.GetAsync("https://api.chucknorris.io/jokes/random");

            if (!response.IsSuccessStatusCode)
            {
                var errorResp = req.CreateResponse(HttpStatusCode.BadGateway);
                await errorResp.WriteStringAsync("Failed to fetch joke");
                return errorResp;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<object>(jsonString);

            var apiResponse = req.CreateResponse(HttpStatusCode.OK);
            await apiResponse.WriteAsJsonAsync(data);

            return apiResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Chuck joke");
            var errorResp = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResp.WriteStringAsync("Server error");
            return errorResp;
        }
    }
}