using System.ComponentModel;
using Spectre.Console.Cli;

namespace AzureCostCli.Commands.ShowCommand;

public class CostByResourceTypeSettings : CostSettings
{
    [CommandOption("--exclude-meter-details")]
    [Description("Exclude meter details from the output.")]
    [DefaultValue(false)]
    public bool ExcludeMeterDetails { get; set; }

    [CommandOption("--resource-type")]
    [Description("The ResourceType to filter. Only the specified ResourceType will be included, by default includes all.")]
    public string ResourceType { get; set; }
}