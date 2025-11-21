using Application.Agents;
using Application.Observability;
using Application.Workflows.ReWoo.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Diagnostics;
using System.Text.Json;

namespace Application.Workflows.ReWoo.Nodes;

public class FlightWorkerNode(IAgent agent) : ReflectingExecutor<FlightWorkerNode>("FlightWorkerNode"), IMessageHandler<OrchestratorWorkerTaskDto>
{
    private List<ChatMessage> _messages = [];


    public async ValueTask HandleAsync(OrchestratorWorkerTaskDto message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        using var activity = Telemetry.Start("FlightWorkerHandleRequest");

        activity?.SetTag("re-woo.node", "flight_worker_node");

        var serialized = JsonSerializer.Serialize(message);

        activity?.SetTag("re-woo.input.message", serialized);

        _messages.Add(new ChatMessage(ChatRole.User, serialized));

        activity?.AddEvent(new ActivityEvent("LLMRequestSent"));

        var response = await agent.RunAsync(_messages, cancellationToken: cancellationToken);

        activity?.AddEvent(new ActivityEvent("LLMResponseReceived"));

        var responseMessage = response.Messages.First();

        _messages.Add(responseMessage);

        activity?.SetTag("re-woo.output.message", response.Messages.First().Text);
    }
}