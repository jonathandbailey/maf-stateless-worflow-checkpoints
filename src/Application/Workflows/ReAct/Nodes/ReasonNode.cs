using Application.Agents;
using Application.Observability;
using Application.Workflows.Events;
using Application.Workflows.ReAct.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Diagnostics;
using System.Text.Json;
using Application.Services;

namespace Application.Workflows.ReAct.Nodes;

public class ReasonNode(IAgent agent, ITravelPlanService travelPlanService) : ReflectingExecutor<ReasonNode>(WorkflowConstants.ReasonNodeName),
   
    IMessageHandler<ActObservation, ActRequest>
{
    private const string StatusThinking = "Evaluating Travel Requirements...";

    public async ValueTask<ActRequest> HandleAsync(
        ActObservation actObservation, 
        IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.Start($"{WorkflowConstants.ReasonNodeName}.observe");

        await context.AddEventAsync(new WorkflowStatusEvent(StatusThinking), cancellationToken);
 
        var message = await Create(actObservation.Message, context, actObservation);

        var input = JsonSerializer.Serialize(actObservation);

        WorkflowTelemetryTags.Preview(activity, WorkflowTelemetryTags.InputNodePreview, input);

        var actRequest = await RunReasoningAsync(message, context, activity, cancellationToken);

        return actRequest;
    }

    private async Task<ChatMessage> Create(string content, IWorkflowContext context, ActObservation observation)
    {

        var travelPlanSummary = await travelPlanService.GetSummary();

        var serialized = JsonSerializer.Serialize(observation);

        var template = $"TravelPlanSummary :{travelPlanSummary}\nObservation :{serialized}";

        return new ChatMessage(ChatRole.User, template);
    }

    private async Task<ActRequest> RunReasoningAsync(ChatMessage message, IWorkflowContext context, Activity? activity,
        CancellationToken cancellationToken)
    {
        var response = await agent.RunAsync(message, cancellationToken);

        WorkflowTelemetryTags.Preview(activity, WorkflowTelemetryTags.OutputNodePreview, response.Text);

        var actRequest = response.Deserialize<ActRequest>(JsonSerializerOptions.Web);

        return actRequest;
    }

}