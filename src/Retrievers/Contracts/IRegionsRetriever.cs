namespace AzureCostCli.Retrievers.Contracts;

public interface IRegionsRetriever
{
    Task<IReadOnlyCollection<AzureRegion>> RetrieveRegions();
}