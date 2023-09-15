using AzureCostCli.DTOs.Responses;

namespace AzureCostCli.Retrievers.Contracts;

public interface IMetricsRetriever
{
    Task RetrieveMetricsForDisk(string diskResourceId);
    Task<MetricsResponse> RetrieveMetricsForResource();
}