using AutomaticToolbox.Console.Commands;
using ConsoleFramework;

const string welcomeMessage =
    "Welcome to AutomaticToolbox! We're thrilled to have you here.\n" +
    "Our application is designed to help you automate the image-generation process with ease, thanks to our powerful AUTOMATIC1111 StableDiffusion API.\n" +
    "To get started, simply type in a command and let AutomaticToolbox handle the rest.\n" +
    "If you're ever unsure about what commands are available, just type \"help\" to access a list of available commands.";

var app = new CliApplication(welcomeMessage);

app.RegisterCommand<PromptGenerateCommand>();
app.RegisterCommand<PromptTemplateCommand>();

app.Run(args);