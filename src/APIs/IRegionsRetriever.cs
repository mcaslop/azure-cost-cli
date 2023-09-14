namespace AzureCostCli.APIs;

public interface IRegionsRetriever
{
    Task<IReadOnlyCollection<AzureRegion>> RetrieveRegions();
}