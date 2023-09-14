using AzureCostCli.CostApi;

namespace AzureCostCli.APIs;

public interface ISubscriptionsRetriever
{
    Task<IReadOnlyCollection<Subscription>> RetrieveAllSubscriptions();
}