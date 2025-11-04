using Application.Agents;
using Application.Workflows.Conversations.Dto;
using Application.Workflows.Conversations.Nodes;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace Application.Workflows.Conversations;

public class Workflow(IAgent reasonAgent, IAgent actAgent)
{
    public async Task Execute(ChatMessage message)
    {
        var inputPort = RequestPort.Create<UserRequest, UserResponse>("user-input");

        var reasonNode = new ReasonNode(reasonAgent);
        var actNode = new ActNode(actAgent);

        var builder = new WorkflowBuilder(reasonNode);

        builder.AddEdge(reasonNode, actNode);
        builder.AddEdge(actNode, inputPort);
        builder.AddEdge(inputPort, actNode);

        var workflow = await builder.BuildAsync<ChatMessage>();

        var run = await InProcessExecution.StreamAsync(workflow, message);

        await foreach (var evt in run.WatchStreamAsync())
        {
            if (evt is ExecutorCompletedEvent executorCompletedEvent)
            {
            }

            if (evt is RequestInfoEvent requestInfoEvent)
            {
            }
        }
    }
}