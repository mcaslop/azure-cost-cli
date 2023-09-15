using System.Net.Http.Json;
using System.Text.Json;
using AzureCostCli.Builders;
using AzureCostCli.DTOs.Responses;
using AzureCostCli.Retrievers.Contracts;
using Spectre.Console;

namespace AzureCostCli.Retrievers;

public class MonitorMetricsRetriever : ApiRetriever, IMetricsRetriever
{
    public MonitorMetricsRetriever(IHttpClientFactory httpClientFactory)
    : base(httpClientFactory.CreateClient("CostApi"))
    {
    }
    public async Task RetrieveMetricsForDisk(string diskResourceId)
    {
        var url = "/batch?api-version=2015-11-01";

        var uri = new Uri(url, UriKind.Relative);
        
        var payload = new
        {
            requests = new [] 
            {
                new {
                    httpMethod = "GET",
                    relativeUrl = $"/{diskResourceId}/providers/microsoft.Insights/metrics?timespan=2023-08-16T00:00:00.000Z/2023-09-15T00:00:00.000Z&interval=FULL&metricnames=Composite Disk Read Bytes%2Fsec&aggregation=average&metricNamespace=microsoft.compute%2Fdisks&validatedimensions=false&api-version=2019-07-01"
                },
                new {
                    httpMethod = "GET",
                    relativeUrl = $"/{diskResourceId}/providers/microsoft.Insights/metrics?timespan=2023-08-15T18:00:00.000Z/2023-09-15T00:00:00.000Z&interval=PT6H&metricnames=Composite Disk Read Bytes%2Fsec&aggregation=average&metricNamespace=microsoft.compute%2Fdisks&autoadjusttimegrain=true&validatedimensions=false&api-version=2019-07-01"
                },
                new {
                    httpMethod = "GET",
                    relativeUrl = $"/{diskResourceId}/providers/microsoft.Insights/metrics?timespan=2023-08-16T00:00:00.000Z/2023-09-15T00:00:00.000Z&interval=FULL&metricnames=Composite Disk Write Bytes%2Fsec&aggregation=average&metricNamespace=microsoft.compute%2Fdisks&validatedimensions=false&api-version=2019-07-01"
                },
                new {
                    httpMethod = "GET",
                    relativeUrl = $"/{diskResourceId}/providers/microsoft.Insights/metrics?timespan=2023-08-15T18:00:00.000Z/2023-09-15T00:00:00.000Z&interval=PT6H&metricnames=Composite Disk Write Bytes%2Fsec&aggregation=average&metricNamespace=microsoft.compute%2Fdisks&autoadjusttimegrain=true&validatedimensions=false&api-version=2019-07-01"
                }
            }
        };

        var response = await ExecuteRequest(true, payload, uri);
        
        var vanillaObject = await response.Content.ReadFromJsonAsync<object?>();

        var rawJson = JsonSerializer.Serialize(vanillaObject);
        
        AnsiConsole.Write(rawJson);
    }

    public async Task<MetricsResponse> RetrieveMetricsForResource()
    {
        var azureResourceUri =
                // Attached
                "/subscriptions/cbc9b442-7c6e-415f-80f9-8f772fa43e9a/resourceGroups/UW2LRGEXCH132/providers/Microsoft.Compute/disks/Admigration12_2008_disk";
                
                // Unattached
                // "/subscriptions/c6e364ad-9cd3-4c26-b358-75a532f60a7f/resourceGroups/UW2PRGELK/providers/Microsoft.Compute/disks/uw2pvmelkdat04_OsDisk_1_7b845cfab72548a5b40df1fb8a8e3754"
            ;

        // var uri = new Uri(
        //     $"{azureResourceUri}/providers/Microsoft.Insights/metrics?api-version=2018-01-01",
        //     UriKind.Relative);

        var metricWithOptionalParamsUri = MetricsUriBuilder
            .New()
            .WithAggregation("Average, count")
            .WithInterval("PT1M")
            .WithMetricNames(new []
            {
                "Composite Disk Read Bytes/sec",
                "Composite Disk Read Operations/sec",
                "Composite Disk Write Bytes/sec",
                "Composite Disk Write Operations/sec",
                "DiskPaidBurstIOPS",
            })
            .WithMetricNamespace("Microsoft.Compute/disks")
            // .WithOrderBy("")
            // .WithResultType(ResultType.Data)
            .ForResource(azureResourceUri)
            .WithTimespan(
                new DateTime(2023, 10, 10, 14, 00, 00), 
                new DateTime(2023, 10, 14, 14, 00, 00))
            .WithTop(10)
            .Build();
        
        var uri = new Uri(
            metricWithOptionalParamsUri,
            // $"{azureResourceUri}/providers/Microsoft.Insights/metrics?api-version=2018-01-01",
            UriKind.Relative);

        var response = await ExecuteRequest(true, null, uri);
        
        var vanillaObject = await response.Content.ReadFromJsonAsync<object?>();

        var rawJson = JsonSerializer.Serialize(vanillaObject);
        MetricsResponse? metricResponse = JsonSerializer.Deserialize<MetricsResponse>(rawJson); 

        return metricResponse ?? null;
    }

}