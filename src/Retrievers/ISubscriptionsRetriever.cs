using AzureCostCli.CostApi;

namespace AzureCostCli.Retrievers;

public interface ISubscriptionsRetriever
{
    Task<IReadOnlyCollection<Subscription>> RetrieveAllSubscriptions();
}