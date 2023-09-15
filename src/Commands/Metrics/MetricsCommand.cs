using AzureCostCli.DTOs.Responses;
using AzureCostCli.OutputFormatters;
using AzureCostCli.Retrievers.Contracts;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AzureCostCli.Commands.Metrics;

public class MetricsCommand : AsyncCommand<MetricsTypeSettings>
{
    private readonly ICostRetriever _costRetriever;
    private readonly IMetricsRetriever _metricsRetriever;
    private readonly ISubscriptionsRetriever _subscriptionsRetriever;
    private readonly Dictionary<OutputFormat, BaseOutputFormatter> _outputFormatters = new();


    public MetricsCommand(
        ICostRetriever costRetriever,
        IMetricsRetriever metricsRetriever,
        ISubscriptionsRetriever subscriptionsRetriever)
    {
        _costRetriever = costRetriever;
        _metricsRetriever = metricsRetriever;
        _subscriptionsRetriever = subscriptionsRetriever;

        // Add the output formatters
        _outputFormatters.Add(OutputFormat.Console, new ConsoleOutputFormatter());
        _outputFormatters.Add(OutputFormat.Json, new JsonOutputFormatter());
        _outputFormatters.Add(OutputFormat.Jsonc, new JsonOutputFormatter());
        _outputFormatters.Add(OutputFormat.Text, new TextOutputFormatter());
        _outputFormatters.Add(OutputFormat.Markdown, new MarkdownOutputFormatter());
        _outputFormatters.Add(OutputFormat.Csv, new CsvOutputFormatter());
    }

    public override ValidationResult Validate(CommandContext context, MetricsTypeSettings settings)
    {
        // // Validate if the timeframe is set to Custom, then the from and to dates must be specified and the from date must be before the to date
        // if (settings.Timeframe == TimeframeType.Custom)
        // {
        //     if (settings.From == null)
        //     {
        //         return ValidationResult.Error("The from date must be specified when the timeframe is set to Custom.");
        //     }

        //     if (settings.To == null)
        //     {
        //         return ValidationResult.Error("The to date must be specified when the timeframe is set to Custom.");
        //     }

        //     if (settings.From > settings.To)
        //     {
        //         return ValidationResult.Error("The from date must be before the to date.");
        //     }
        // }

        return ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, MetricsTypeSettings settings)
    {
        // Show version
        if (settings.Debug)
            AnsiConsole.WriteLine($"Version: {typeof(MetricsCommand).Assembly.GetName().Version}");

        var diskResourceId =
            "subscriptions/cbc9b442-7c6e-415f-80f9-8f772fa43e9a/resourceGroups/UW2LRGEXCH132/providers/Microsoft.Compute/disks/2008_r2_image_disk";
        
        // MetricsResponse metricResponse = null;
        // await AnsiConsole
        //     .Status()
        //     .StartAsync($"Fetching metrics ...", async ctx =>
        //     {
        //         metricResponse = await _metricsRetriever.RetrieveMetricsForResource();
        //     }); 
        //
        // // Write the output
        // JsonOutputFormatter.WriteJson(settings, metricResponse);
        
        await AnsiConsole
            .Status()
            .StartAsync($"Fetching metrics ...", async ctx =>
            {
                await _metricsRetriever.RetrieveMetricsForDisk(diskResourceId);
            }); 

        return 0;
    }
}