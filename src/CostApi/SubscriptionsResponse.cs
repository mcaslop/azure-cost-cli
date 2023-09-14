namespace AzureCostCli.CostApi;

public class SubscriptionsResponse
{
    public Subscription[] value { get; set; }

    public CountObject count { get; set; }
}

public class CountObject
{
    public string type { get; set; }
    public int value { get; set; }
}