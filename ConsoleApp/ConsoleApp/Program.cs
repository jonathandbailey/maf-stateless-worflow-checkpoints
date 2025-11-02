using ConsoleApp.Services;
using ConsoleApp.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);


builder.Configuration.AddUserSecrets<Program>();

builder.Services.Configure<LanguageModelSettings>(
    builder.Configuration.GetSection("LanguageModelSettings"));

builder.Services.AddSingleton<IPromptService, PromptService>();

builder.Services.AddHostedService<Application>();

var host = builder.Build();
await host.RunAsync();







