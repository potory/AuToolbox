using AuToolbox.Console.Commands;
using AuToolbox.Core.Configurations;
using AuToolbox.Core.Functions;
using ConsoleFramework;
using Microsoft.Extensions.DependencyInjection;
using Prompthing.Core;
using Prompthing.Core.Functions;
using Prompthing.Core.Templates;
using SonScript.Core;

const string welcomeMessage =
    "Welcome to AuToolbox!\n" +
    "This application is designed to help you automate the image-generation process with AUTOMATIC1111 StableDiffusion API.\n" +
    "To get started, simply type in a command and let AuToolbox handle the rest.\n" +
    "If you're ever unsure about what commands are available, just type \"help\" to access a list of available commands.";

var app = new CliApplication(welcomeMessage);

app.ServiceCollection
    .AddSingleton(x => x)
    .AddSingleton<FunctionContext>()
    .AddSingleton<FunctionParser>()
    .AddSingleton<ReferencePool>()
    .AddSingleton<DatasetCompiler>()
    .AddTransient<ConfigMapper>()
    .AddSingleton(CreateFunctionFactory);

app.RegisterCommand<PromptGenerateCommand>();
app.RegisterCommand<PromptTemplateCommand>();
app.RegisterCommand<ImagesGenerateCommand>();
app.RegisterCommand<ClearCacheCommand>();

await app.Run(args);

FunctionFactory CreateFunctionFactory(IServiceProvider serviceProvider)
{
    void RegisterFunctions(FunctionFactory functionFactory)
    {
        functionFactory.RegisterFunction<TemplateFunction>("t");
        functionFactory.RegisterFunction<TemplateFunction>("temp");
        functionFactory.RegisterFunction<TemplateFunction>("template");
        functionFactory.RegisterFunction<BackspaceFunction>("backspace");
        functionFactory.RegisterFunction<PrompthingFunction>("prompthing");
        functionFactory.RegisterFunction<PreviousResultFunction>("previousResult");
        functionFactory.RegisterFunction<MultipleFromCategory>("multcat");
    }

    var factory = new FunctionFactory(serviceProvider.GetService<IServiceProvider>());
    RegisterFunctions(factory);
    return factory;
}