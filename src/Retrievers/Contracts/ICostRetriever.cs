using AzureCostCli.Commands;
using AzureCostCli.CostApi;

namespace AzureCostCli.Retrievers.Contracts;

public interface ICostRetriever
{
    Task<Subscription> RetrieveSubscription(bool includeDebugOutput, Guid subscriptionId);
    
    Task<IEnumerable<CostItem>> RetrieveCosts(bool includeDebugOutput, Guid subscriptionId,
        string[] filter,MetricType metric,
        TimeframeType timeFrame, DateOnly from, DateOnly to);

    Task<IEnumerable<CostNamedItem>> RetrieveCostByServiceName(bool includeDebugOutput,
        Guid subscriptionId, string[] filter, MetricType metric,TimeframeType timeFrame, DateOnly from, DateOnly to);

    Task<IEnumerable<CostNamedItem>> RetrieveCostByLocation(bool includeDebugOutput, Guid subscriptionId,
        string[] filter,MetricType metric,
        TimeframeType timeFrame, DateOnly from, DateOnly to);

    Task<IEnumerable<CostNamedItem>> RetrieveCostByResourceGroup(bool includeDebugOutput, Guid subscriptionId,
        string[] filter,MetricType metric,
        TimeframeType timeFrame, DateOnly from, DateOnly to);

    Task<IEnumerable<CostItem>> RetrieveForecastedCosts(bool includeDebugOutput, Guid subscriptionId, 
        string[] filter,MetricType metric,
        TimeframeType timeFrame, DateOnly from, DateOnly to);
    Task<IEnumerable<CostResourceItem>> RetrieveCostForResources(bool settingsDebug, Guid subscriptionId, string[] filter, MetricType metric,
        bool excludeMeterDetails,TimeframeType settingsTimeframe, DateOnly from, DateOnly to);
    Task<IEnumerable<CostResourceItem>> RetrieveCostForResourceTypes(bool settingsDebug, Guid subscriptionId, string[] filter, MetricType metric,
        bool excludeMeterDetails,TimeframeType settingsTimeframe, DateOnly from, DateOnly to, string resourceType,
        string subscriptionName = null);
    Task<IEnumerable<BudgetItem>> RetrieveBudgets(bool settingsDebug, Guid subscriptionId);
    Task<IEnumerable<CostDailyItem>> RetrieveDailyCost(bool settingsDebug, Guid subscriptionId, string[] filter,MetricType metric, string dimension, TimeframeType settingsTimeframe, DateOnly settingsFrom, DateOnly settingsTo);
}



