using AzureCostCli.CostApi;
using AzureCostCli.OutputFormatters;
using AzureCostCli.Retrievers.Contracts;
using Spectre.Console;
using Spectre.Console.Cli;

namespace AzureCostCli.Commands.DeepDive;

public class DeepDiveCommand : AsyncCommand<DeepDiveTypeSettings>
{
    private readonly ICostRetriever _costRetriever;
    private readonly ISubscriptionsRetriever _subscriptionsRetriever;
    private readonly Dictionary<OutputFormat, BaseOutputFormatter> _outputFormatters = new();

    public DeepDiveCommand(ISubscriptionsRetriever subscriptionsRetriever, ICostRetriever costRetriever)
    {
        _subscriptionsRetriever = subscriptionsRetriever;
        _costRetriever = costRetriever;

        // Add the output formatters
        _outputFormatters.Add(OutputFormat.Console, new ConsoleOutputFormatter());
        _outputFormatters.Add(OutputFormat.Json, new JsonOutputFormatter());
        _outputFormatters.Add(OutputFormat.Jsonc, new JsonOutputFormatter());
        _outputFormatters.Add(OutputFormat.Text, new TextOutputFormatter());
        _outputFormatters.Add(OutputFormat.Markdown, new MarkdownOutputFormatter());
        _outputFormatters.Add(OutputFormat.Csv, new CsvOutputFormatter());
    }

    public override ValidationResult Validate(CommandContext context, DeepDiveTypeSettings settings)
    {
        // Validate if the timeframe is set to Custom, then the from and to dates must be specified and the from date must be before the to date
        if (settings.Timeframe == TimeframeType.Custom)
        {
            if (settings.From == null)
            {
                return ValidationResult.Error("The from date must be specified when the timeframe is set to Custom.");
            }

            if (settings.To == null)
            {
                return ValidationResult.Error("The to date must be specified when the timeframe is set to Custom.");
            }

            if (settings.From > settings.To)
            {
                return ValidationResult.Error("The from date must be before the to date.");
            }
        }

        return ValidationResult.Success();
    }

    public override async Task<int> ExecuteAsync(CommandContext context, DeepDiveTypeSettings settings)
    {
        // Show version
        if (settings.Debug)
            AnsiConsole.WriteLine($"Version: {typeof(DeepDiveCommand).Assembly.GetName().Version}");

        IEnumerable<Subscription> subscriptions = new List<Subscription>();
        // Fetch the costs from the Azure Cost Management API
        var resources = new List<CostResourceItem>();

       if (settings.Subscription == Guid.Empty)
       {
            await AnsiConsole
                .Status()
                .StartAsync("Fetching all Tenant's subscriptions...", async ctx =>
                {
                    subscriptions = await _subscriptionsRetriever.RetrieveAllSubscriptions();
                });

            foreach(var sub in subscriptions.OrderBy(s => s.displayName))
            {
                var subscriptionId = Guid.Parse(sub.subscriptionId);

                await AnsiConsole
                    .Status()
                    .StartAsync($"Fetching cost data for resources for subscription ({sub.displayName})...", async ctx =>
                    {
                        List<CostResourceItem> subResources = (await _costRetriever.RetrieveCostForResourceTypes(
                            settings.Debug,
                            subscriptionId,
                            settings.Filter,
                            settings.Metric,
                            settings.ExcludeMeterDetails,
                            settings.Timeframe,
                            settings.From,
                            settings.To,
                            settings.ResourceType,
                            sub.displayName)).ToList();

                        resources.AddRange(subResources);
                    });
            }
       }
       else
       {
            Subscription singleSubscription = null;

            await AnsiConsole
                .Status()
                .StartAsync("Fetching single subscription...", async ctx =>
                {
                    singleSubscription = await _costRetriever.RetrieveSubscription(settings.Debug, settings.Subscription);
                });

            await AnsiConsole.Status()
                .StartAsync("Fetching cost data for resources...", async ctx =>
                {
                    List<CostResourceItem> singleSubsResources = (await _costRetriever.RetrieveCostForResourceTypes(
                        settings.Debug,
                        Guid.Parse(singleSubscription.subscriptionId),
                        settings.Filter,
                        settings.Metric,
                        settings.ExcludeMeterDetails,
                        settings.Timeframe,
                        settings.From,
                        settings.To,
                        settings.ResourceType,
                        singleSubscription.displayName)).ToList();

                    resources.AddRange(singleSubsResources);
                });
       }        

        // Write the output
        await _outputFormatters[settings.Output]
            .WriteDeepDive(settings, resources);

        return 0;
    }
}