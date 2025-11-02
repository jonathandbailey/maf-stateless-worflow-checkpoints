using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace ConsoleApp.Workflows.ReAct;

public class ReasonNode(AIAgent agent) : ReflectingExecutor<ReasonNode>("ReasonNode"), IMessageHandler<string, string>
{
    public async ValueTask<string> HandleAsync(string message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var response = await agent.RunAsync(new ChatMessage(ChatRole.User, message), cancellationToken: cancellationToken);

        return response.Text;
    }
}