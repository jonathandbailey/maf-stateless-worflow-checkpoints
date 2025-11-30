using Application.Agents;
using Application.Observability;
using Application.Workflows.ReAct.Dto;
using Application.Workflows.ReWoo.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using System.Diagnostics;
using Application.Workflows.Events;
using Microsoft.Agents.AI;

namespace Application.Workflows.ReAct.Nodes;

public class ActNode(IAgent agent) : ReflectingExecutor<ActNode>(WorkflowConstants.ActNodeName), IMessageHandler<ActRequest>, 
    IMessageHandler<UserResponse>
{
    private const string NoJsonReturnedByAgent = "Agent/LLM did not return formnatted JSON for routing/actions";
    private const string AgentJsonParseFailed = "Agent JSON parse failed";

    private Activity? _activity;
 
    public async ValueTask HandleAsync(ActRequest request, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        TraceActRequest(request);

        var userId = await context.UserId();
        var sessionId = await context.SessionId();
    
        var response = await agent.RunAsync(request.Message, sessionId, userId, cancellationToken);

        TraceAgentRequestSent(response);

        if (!JsonOutputParser.HasJson(response.Text))
        {
            await context.AddEventAsync(new TravelWorkflowErrorEvent(NoJsonReturnedByAgent,response.Text, WorkflowConstants.ActNodeName), cancellationToken);
            return;
        }
        
        RouteAction routeAction;
        try
        {
            routeAction = JsonOutputParser.Parse<RouteAction>(response.Text);
        }
        catch (Exception ex)
        {
            await context.AddEventAsync(
                new TravelWorkflowErrorEvent(AgentJsonParseFailed, response.Text, WorkflowConstants.ActNodeName, ex),
                cancellationToken);
            return;
        }

        var cleanedResponse = JsonOutputParser.Remove(response.Text);

        _activity?.SetTag("workflow.route.message", cleanedResponse);
        _activity?.SetTag("workflow.route", routeAction.Route);

        switch (routeAction.Route)
        {
            case "ask_user":
                await context.SendMessageAsync(new UserRequest(cleanedResponse), cancellationToken: cancellationToken);
                break;
            case "complete":
                await context.AddEventAsync(new ReasonActWorkflowCompleteEvent(cleanedResponse), cancellationToken);
                break;
            case "orchestrate":
            {
                var extractedJson = JsonOutputParser.ExtractJson(response.Text);

                await context.SendMessageAsync(new OrchestrationRequest(extractedJson), cancellationToken: cancellationToken);
                break;
            }
            default:
                await context.AddEventAsync(
                    new TravelWorkflowErrorEvent($"Unknown route '{routeAction.Route}' returned by agent", cleanedResponse, WorkflowConstants.ActNodeName),
                    cancellationToken);
                break;
        }

        TraceEnd();
    }

    public async ValueTask HandleAsync(UserResponse userResponse, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.Start($"{WorkflowConstants.ActNodeName}.handleUserResponse");

        WorkflowTelemetryTags.SetPreview(activity, userResponse.Message);

        await context.SendMessageAsync(new ActObservation(userResponse.Message), cancellationToken: cancellationToken);
    }

    private void TraceActRequest(ActRequest request)
    {
        _activity = Telemetry.Start($"{WorkflowConstants.ActNodeName}.handleRequest");

        _activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.ActNodeName);

        WorkflowTelemetryTags.SetPreview(_activity, request.Message.Text);
    }

    private void TraceAgentRequestSent(AgentRunResponse response)
    {
        WorkflowTelemetryTags.SetPreview(_activity, response.Text);
    }

    private void TraceEnd()
    {
        _activity?.Dispose();
    }
}

