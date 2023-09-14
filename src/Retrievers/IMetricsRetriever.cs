namespace AzureCostCli.Retrievers;

public interface IMetricsRetriever
{
    Task RetrieveMetricsForResource();
}