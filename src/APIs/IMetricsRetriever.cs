namespace AzureCostCli.APIs;

public interface IMetricsRetriever
{
    Task RetrieveMetricsForResource();
}