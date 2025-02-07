using Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts;
using Gemstar.Tools.DotNetCoreConvert.Console.ProjectConverts.CsprojFile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.CommandLine.Invocation;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfiguration configuration = builder.Build();

var services = new ServiceCollection();
services.AddOptions();
services.Configure<FrameworkPackageMappingOption>(configuration.GetSection("FrameworkPackageMappingOption"));

var serviceProvider = services.BuildServiceProvider();

var argProjectPathSource = new Argument<string>("projectPathSource", "The path to the .NET Framework project to migrate");
var argProjectPathDestination = new Option<string>("--projectPathTarget", "The path to the .NET Core/.NET 8+ project to create. the default value is {projectPathSource}.Core if it is not specified");
var rootCommand = new RootCommand
{
    argProjectPathSource,
    argProjectPathDestination,
};

rootCommand.Description = "A tool to migrate .NET Framework projects to .NET Core/.NET 8+.";

rootCommand.SetHandler(async (InvocationContext context) =>
{
    var projectPathSource = context.ParseResult.GetValueForArgument(argProjectPathSource);
    var projectPathTarget = context.ParseResult.GetValueForOption(argProjectPathDestination);
    if (string.IsNullOrWhiteSpace(projectPathTarget))
    {
        projectPathTarget = $"{projectPathSource}.Core";
    }
    //check the projectPathSource is valid
    if (!Directory.Exists(projectPathSource))
    {
        Console.WriteLine($"The project path {projectPathSource} does not exist.");
        return;
    }
    var projectConvert = new ProjectConvert(serviceProvider, projectPathSource, projectPathTarget);
    await projectConvert.ConvertAsync();

    // Get the value of the projectPath argument
});

return await rootCommand.InvokeAsync(args);