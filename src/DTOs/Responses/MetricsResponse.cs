namespace AzureCostCli.DTOs.Responses;

public record MetricsResponse(int cost, string timespan, string interval, GG value, string @namespace,
    string resourceregion);

public record GG (string id, string type, HHH name, string displayDescription, string unit, object[] timeseries,
    string errorCode);

public record HHH(string value, string localizedValue);
