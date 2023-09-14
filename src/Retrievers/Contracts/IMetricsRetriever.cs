using AzureCostCli.DTOs;
using AzureCostCli.DTOs.Responses;

namespace AzureCostCli.Retrievers.Contracts;

public interface IMetricsRetriever
{
    Task<MetricsResponse> RetrieveMetricsForResource();
}