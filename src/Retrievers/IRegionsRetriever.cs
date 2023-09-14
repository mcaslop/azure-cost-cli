namespace AzureCostCli.Retrievers;

public interface IRegionsRetriever
{
    Task<IReadOnlyCollection<AzureRegion>> RetrieveRegions();
}