using System.Diagnostics;
using Application.Agents;
using Application.Observability;
using Application.Workflows.ReWoo.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Text.Json;

namespace Application.Workflows.ReWoo.Nodes;

public class HotelWorkerNode(IAgent agent) : ReflectingExecutor<HotelWorkerNode>(WorkflowConstants.HotelWorkerNodeName), IMessageHandler<OrchestratorWorkerTaskDto>
{
    private Activity? _activity;
    
    public async ValueTask HandleAsync(OrchestratorWorkerTaskDto message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Trace(message);
  
        var serialized = JsonSerializer.Serialize(message);

        _activity?.SetTag("re-woo.input.message", serialized);

        var userId = await context.UserId();
        var sessionId = await context.SessionId();

        var response = await agent.RunAsync(new ChatMessage(ChatRole.User, serialized), sessionId, userId, cancellationToken: cancellationToken);
    
        var responseMessage = response.Messages.First();

        _activity?.SetTag("re-woo.output.message", response.Messages.First().Text);

        await context.SendMessageAsync(new ArtifactStorageDto(message.ArtifactKey, responseMessage.Text), cancellationToken: cancellationToken);

        TraceEnd();
    }

    private void Trace(OrchestratorWorkerTaskDto message)
    { 
        _activity = Telemetry.Start($"{WorkflowConstants.HotelWorkerNodeName}.handleRequest");

        _activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.HotelWorkerNodeName);

        _activity?.SetTag("re-woo.input.message", message);
    }

    private void TraceEnd()
    {
        _activity?.Dispose();
    }
}