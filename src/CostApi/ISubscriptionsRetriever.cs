using AzureCostCli.Commands;

namespace AzureCostCli.CostApi;

public interface ISubscriptionsRetriever
{
    Task<IReadOnlyCollection<Subscription>> RetrieveAllSubscriptions();
}