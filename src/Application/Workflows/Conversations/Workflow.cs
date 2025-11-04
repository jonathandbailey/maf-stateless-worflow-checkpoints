using Application.Agents;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace Application.Workflows.Conversations;

public class Workflow(IAgent reasonAgent, IAgent actAgent)
{
    public async Task Execute(ChatMessage message)
    {
        var reasonNode = new ReasonNode(reasonAgent);

        var builder = new WorkflowBuilder(reasonNode);
     
        var workflow = await builder.BuildAsync<ChatMessage>();

        var run = await InProcessExecution.StreamAsync(workflow, message);

        await foreach (var evt in run.WatchStreamAsync())
        {
            if (evt is ExecutorCompletedEvent executorCompletedEvent)
            {
            }
        }
    }
}