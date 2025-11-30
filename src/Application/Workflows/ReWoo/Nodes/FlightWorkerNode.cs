using Application.Agents;
using Application.Observability;
using Application.Workflows.Events;
using Application.Workflows.ReWoo.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Diagnostics;
using System.Text.Json;

namespace Application.Workflows.ReWoo.Nodes;

public class FlightWorkerNode(IAgent agent) : 
    ReflectingExecutor<FlightWorkerNode>(WorkflowConstants.FlightWorkerNodeName), 
    IMessageHandler<OrchestratorWorkerTaskDto>
{
    private const string FlightWorkerNodeError = "Flight Worker Node has failed to execute.";

    private Activity? _activity;
    
    public async ValueTask HandleAsync(OrchestratorWorkerTaskDto message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        Trace(message);

        try
        {
            var serialized = JsonSerializer.Serialize(message);
    
            var userId = await context.UserId();
            var sessionId = await context.SessionId();

            var response = await agent.RunAsync( new ChatMessage(ChatRole.User, serialized), sessionId, userId, cancellationToken: cancellationToken);
    
            var responseMessage = response.Messages.First();

            WorkflowTelemetryTags.SetPreview(_activity, responseMessage.Text);

            _activity?.SetTag(WorkflowTelemetryTags.ArtifactKey, message.ArtifactKey);

            await context.SendMessageAsync(new ArtifactStorageDto(message.ArtifactKey, responseMessage.Text), cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            await context.AddEventAsync(new TravelWorkflowErrorEvent(FlightWorkerNodeError, message.ArtifactKey, WorkflowConstants.FlightWorkerNodeName,exception), cancellationToken);
        }

        TraceEnd();
    }

    private void Trace(OrchestratorWorkerTaskDto message)
    {
        _activity = Telemetry.Start($"{WorkflowConstants.FlightWorkerNodeName}.handleRequest");

        _activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.FlightWorkerNodeName);

        WorkflowTelemetryTags.SetPreview(_activity, JsonSerializer.Serialize(message));
    }

    private void TraceEnd()
    {
        _activity?.Dispose();
    }
}