using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using AzureCostCli.DTOs;
using AzureCostCli.DTOs.Responses;
using AzureCostCli.Retrievers.Contracts;
using Spectre.Console;
using Spectre.Console.Json;

namespace AzureCostCli.Retrievers;

public class MonitorMetricsRetriever : IMetricsRetriever
{
    private readonly HttpClient _client;
    private bool _tokenRetrieved;

    public MonitorMetricsRetriever(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("CostApi");
    }

    private async Task RetrieveToken(bool includeDebugOutput)
    {
        if (_tokenRetrieved)
            return;

        // Get the token by using the DefaultAzureCredential
        var tokenCredential = new ChainedTokenCredential(
            new AzureCliCredential(),
            new DefaultAzureCredential());

        if (includeDebugOutput)
            AnsiConsole.WriteLine($"Using token credential: {tokenCredential.GetType().Name} to fetch a token.");

        var token = await tokenCredential.GetTokenAsync(new TokenRequestContext(new[]
            { $"https://management.azure.com/.default" }));

        if (includeDebugOutput)
            AnsiConsole.WriteLine($"Token retrieved and expires at: {token.ExpiresOn}");

        // Set as the bearer token for the HTTP client
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        _tokenRetrieved = true;
    }

    private async Task<HttpResponseMessage> ExecuteToCallApi(bool includeDebugOutput, object? payload, Uri uri)
    {
        await RetrieveToken(includeDebugOutput);

        if (includeDebugOutput)
        {
            AnsiConsole.WriteLine($"Retrieving data from {uri} using the following payload:");
            AnsiConsole.Write(new JsonText(JsonSerializer.Serialize(payload)));
            AnsiConsole.WriteLine();
        }

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var response = payload == null
            ? await _client.GetAsync(uri)
            : await _client.PostAsJsonAsync(uri, payload, options);

        if (includeDebugOutput)
        {
            AnsiConsole.WriteLine(
                $"Response status code is {response.StatusCode} and got payload size of {response.Content.Headers.ContentLength}");

            // if (!response.IsSuccessStatusCode)
            // {
            //     AnsiConsole.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");
            // }

            var responsess = await response.Content.ReadFromJsonAsync<object?>();
            var ttt = JsonSerializer.Serialize(responsess);
            AnsiConsole.WriteLine($"Response content: {ttt}");
        }

        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<MetricsResponse> RetrieveMetricsForResource()
    {
        var azureResourceUri =
            // "/subscriptions/cbc9b442-7c6e-415f-80f9-8f772fa43e9a/resourceGroups/UW2LRGEXCH132/providers/Microsoft.Compute/disks/Admigration12_2008_disk";
            "/subscriptions/c6e364ad-9cd3-4c26-b358-75a532f60a7f/resourceGroups/UW2PRGELK/providers/Microsoft.Compute/disks/uw2pvmelkdat04_OsDisk_1_7b845cfab72548a5b40df1fb8a8e3754";

        // var uri = new Uri(
        //     $"{azureResourceUri}/providers/Microsoft.Insights/metrics?api-version=2018-01-01",
        //     UriKind.Relative);

        var uri = new Uri(
            $"{azureResourceUri}/providers/Microsoft.Insights/metrics?api-version=2018-01-01",
            UriKind.Relative);

        var response = await ExecuteToCallApi(true, null, uri);

        var metricResponse = await response.Content.ReadFromJsonAsync<MetricsResponse>();

        return metricResponse;
    }

}