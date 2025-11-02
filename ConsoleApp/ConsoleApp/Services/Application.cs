using System.ClientModel;
using Azure.AI.OpenAI;
using ConsoleApp.Settings;
using ConsoleApp.Workflows.ReAct;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OpenAI;

namespace ConsoleApp.Services;

public class Application(IOptions<LanguageModelSettings> settings, IPromptService promptService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var chatClient = new AzureOpenAIClient(new Uri(settings.Value.EndPoint),
                new ApiKeyCredential(
                    settings.Value.ApiKey))
            .GetChatClient(settings.Value.DeploymentName);

        var agent = chatClient.CreateAIAgent(new ChatClientAgentOptions
        {
            Instructions = promptService.GetPrompt("Reason Prompt")
        });

        var reasonNode = new ReasonNode(agent);

        var builder = new WorkflowBuilder(reasonNode);

        var workflow = builder.Build();
     
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("You: ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                continue;
            }

            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            await using var run = await InProcessExecution.RunAsync(workflow, userInput, cancellationToken: cancellationToken);
            
            foreach (var evt in run.NewEvents)
            {
                switch (evt)
                {
                    case ExecutorCompletedEvent executorComplete:
                        Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
                        break;
                    case WorkflowOutputEvent workflowOutput:
                        Console.WriteLine($"Workflow '{workflowOutput.SourceId}' outputs: {workflowOutput.Data}");
                        break;
                }
            }

            Console.WriteLine();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}