using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Azure.Identity;
using Spectre.Console;
using Spectre.Console.Json;

namespace AzureCostCli.Retrievers;

public abstract class ApiRetriever
{
    protected readonly HttpClient Client;

    private bool _tokenRetrieved;

    public ApiRetriever(HttpClient client)
    {
        Client = client;
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
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

        _tokenRetrieved = true;
    }

    protected async Task<HttpResponseMessage> ExecuteRequest(bool includeDebugOutput, object? payload, Uri uri)
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
            ? await Client.GetAsync(uri)
            : await Client.PostAsJsonAsync(uri, payload, options);

        if (includeDebugOutput)
        {
            AnsiConsole.WriteLine(
                $"Response status code is {response.StatusCode} and got payload size of {response.Content.Headers.ContentLength}");

            if (!response.IsSuccessStatusCode)
            {
                AnsiConsole.WriteLine($"Response content (failed): {await response.Content.ReadAsStringAsync()}");
            }
            else
            {
                AnsiConsole.WriteLine($"Response content (successful): {await response.Content.ReadAsStringAsync()}");
            }
        }

        response.EnsureSuccessStatusCode();
        return response;
    }
}

