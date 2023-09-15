using System.Text;

namespace AzureCostCli.Builders;

public class MetricsUriBuilder
{
    public static MetricsUriBuilder New()
    {
        return new MetricsUriBuilder();
    }

    private MetricsUriBuilder()
    {
    }

    /*
     * GET https://management.azure.com/
     * {resourceUri}/providers/Microsoft.Insights/metrics
     * ?timespan={timespan}
     * &interval={interval}
     * &metricnames={metricnames}
     * &aggregation={aggregation}
     * &top={top}
     * &orderby={orderby}
     * &$filter={$filter}
     * &resultType={resultType}
     * &api-version=2018-01-01
     * &metricnamespace={metricnamespace}
     */
    private string ResourceUri;

    public MetricsUriBuilder ForResource(string resourceUri)
    {
        ResourceUri = resourceUri;
        return this;
    }

    private DateTime? FromDate;
    private DateTime? ToDate;

    public MetricsUriBuilder WithTimespan(DateTime fromDate, DateTime toDate)
    {
        FromDate = fromDate;
        ToDate = toDate;
        return this;
    }

    private string Interval;

    public MetricsUriBuilder WithInterval(string interval)
    {
        Interval = interval;
        return this;
    }

    private IEnumerable<string> MetricNames;

    public MetricsUriBuilder WithMetricNames(IEnumerable<string> metricNames)
    {
        MetricNames = metricNames;
        return this;
    }

    private string Aggregation;

    public MetricsUriBuilder WithAggregation(string aggregation)
    {
        Aggregation = aggregation;
        return this;
    }

    private int Top;

    public MetricsUriBuilder WithTop(int top)
    {
        Top = top;
        return this;
    }

    private string OrderBy;

    public MetricsUriBuilder WithOrderBy(string orderBy)
    {
        OrderBy = orderBy;
        return this;
    }

    // filter goes here

    private ResultType? ResultType;

    public MetricsUriBuilder WithResultType(ResultType resultType)
    {
        ResultType = resultType;
        return this;
    }

    private string MetricNamespace;

    public MetricsUriBuilder WithMetricNamespace(string metricNamespace)
    {
        MetricNamespace = metricNamespace;
        return this;
    }

    private const string ISO_DATE_FORMAT = "yyyy-MM-dd";
    public string Build()
    {
        var uri = new StringBuilder();

        if (ResourceUri is null)
            throw new ArgumentNullException("ResourceUri is a required component of the URI.");
        
        uri.Append($"{ResourceUri}/providers/Microsoft.Insights/metrics");

        if(FromDate.HasValue && ToDate.HasValue)
        {
            var fromDate = FromDate.Value.ToString("s"); //"yyyy-MM-dd");
            var toDate = ToDate.Value.ToString("s"); // "yyyy-MM-dd");
            uri.Append($"?timespan={fromDate}Z/{toDate}Z");
        } 
        
        if(Interval is not null)
            uri.Append($"&interval={Interval}");
        
        if(MetricNames is not null && MetricNames.Any())
            uri.Append($"&metricnames={string.Join(",", MetricNames)}");

        if(Aggregation is not null)
            uri.Append($"&aggregation={Aggregation}");
        
        uri.Append($"&top={(Top == 0 ? 10 : Top)}");
        
        if(OrderBy is not null)
            uri.Append($"&orderby={OrderBy}");
        
        // uri.Append($"&$filter={$filter}");
        if(ResultType.HasValue)
            uri.Append($"&resultType={ResultType}");
        
        uri.Append($"&api-version=2018-01-01");
        uri.Append($"&metricnamespace={MetricNamespace}");

        return uri.ToString();
    }
}

public enum ResultType
{
    Data,
    Metadata
}