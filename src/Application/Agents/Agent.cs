using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Application.Agents;

public class Agent(AIAgent agent) : IAgent
{
    public async Task<AgentRunResponse> RunAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await agent.RunAsync(messages, thread, options, cancellationToken);
    }
}
public interface IAgent
{
    Task<AgentRunResponse> RunAsync(IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);
}