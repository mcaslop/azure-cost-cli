using AzureCostCli.DTOs;

namespace AzureCostCli.Retrievers.Contracts;

public interface IMetricsRetriever
{
    Task<MetricsResponse> RetrieveMetricsForResource();
}