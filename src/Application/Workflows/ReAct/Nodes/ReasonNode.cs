using Application.Agents;
using Application.Observability;
using Application.Workflows.Events;
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
    private const string StatusThinking = "Evaluating Travel Requirements...";

    public async ValueTask<ActRequest> HandleAsync(
        TravelWorkflowRequestDto requestDto, 
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.Start($"{WorkflowConstants.ReasonNodeName}.handleRequest");

        await context.AddEventAsync(new WorkflowStatusEvent(StatusThinking), cancellationToken);

        var chatMessage = await Create(requestDto.Message.Text, context);

        activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.ReasonNodeName);

        WorkflowTelemetryTags.Preview(activity, WorkflowTelemetryTags.InputNodePreview, chatMessage.Text);

        var actRequest = await RunReasoningAsync(chatMessage, context, activity, cancellationToken);

        WorkflowTelemetryTags.Preview(activity, WorkflowTelemetryTags.OutputNodePreview, actRequest.Message.Text);

        return actRequest;
    }

    public async ValueTask<ActRequest> HandleAsync(
        ActObservation actObservation, 
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.Start($"{WorkflowConstants.ReasonNodeName}.observe");

        await context.AddEventAsync(new WorkflowStatusEvent(StatusThinking), cancellationToken);
 
        var message = await Create(actObservation.Message, context);

        WorkflowTelemetryTags.Preview(activity, WorkflowTelemetryTags.InputNodePreview, message.Text);

        var actRequest = await RunReasoningAsync(message, context, activity, cancellationToken);

        WorkflowTelemetryTags.Preview(activity, WorkflowTelemetryTags.OutputNodePreview, actRequest.Message.Text);

        return actRequest;
    }

    private async Task<ChatMessage> Create(string content, IWorkflowContext context)
    {
        var state = await context.ReasonState();

        var template = $"user input :{content}\nstate :{state}";

        return new ChatMessage(ChatRole.User, template);
    }

    private async Task<ActRequest> RunReasoningAsync(ChatMessage message, IWorkflowContext context, Activity? activity, CancellationToken cancellationToken)
    {
        var response = await agent.RunAsync(message, cancellationToken);
      
        return new ActRequest(response.Messages.First());
    }

    private static void Annotate(Activity? activity, string preview)
    {
        if (activity == null) return;

        activity.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.ReasonNodeName);
        WorkflowTelemetryTags.SetInputPreview(activity, preview);
    }
}