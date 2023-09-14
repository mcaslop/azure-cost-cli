using AzureCostCli.CostApi;

namespace AzureCostCli.Retrievers.Contracts;

public interface ISubscriptionsRetriever
{
    Task<IReadOnlyCollection<Subscription>> RetrieveAllSubscriptions();
}