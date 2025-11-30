using Application.Agents;
using Application.Observability;
using Application.Workflows.ReAct.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Diagnostics;

namespace Application.Workflows.ReAct.Nodes;

public class ReasonNode(IAgent agent) : ReflectingExecutor<ReasonNode>(WorkflowConstants.ReasonNodeName),
    IMessageHandler<TravelWorkflowRequestDto, ActRequest>,
    IMessageHandler<ActObservation, ActRequest>
{
    private Activity? _activity;
    public async ValueTask<ActRequest> HandleAsync(TravelWorkflowRequestDto requestDto, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        Trace(requestDto);

        await context.SessionId(requestDto.SessionId);
        await context.UserId(requestDto.UserId);

        var response = await Process(requestDto.Message, context, cancellationToken);

        TraceEnd();

        return response;
    }

    public async ValueTask<ActRequest> HandleAsync(ActObservation actObservation, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using var activity = Telemetry.Start($"{WorkflowConstants.ReasonNodeName}.observe");

        activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.ReasonNodeName);

        WorkflowTelemetryTags.SetPreview(activity, actObservation.Message);

        var message = new ChatMessage(ChatRole.User, actObservation.Message);

        return await Process(message, context, cancellationToken);
    }

    private async Task<ActRequest> Process(ChatMessage message, IWorkflowContext context, CancellationToken cancellationToken)
    {
        var userId = await context.UserId();
        var sessionId = await context.SessionId();

        var response = await agent.RunAsync(message, sessionId, userId, cancellationToken);
  
        WorkflowTelemetryTags.SetPreview(_activity, response.Messages.First().Text);

        return new ActRequest(response.Messages.First());
    }

    private void Trace(TravelWorkflowRequestDto requestDto)
    {
        _activity = Telemetry.Start($"{WorkflowConstants.ReasonNodeName}.handleRequest");

        _activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.ReasonNodeName);

        WorkflowTelemetryTags.SetPreview(_activity, requestDto.Message.Text);
    }

    private void TraceEnd()
    {
        _activity?.Dispose();
    }
}