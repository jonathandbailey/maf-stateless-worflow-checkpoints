using Application.Agents;
using Application.Workflows.Events;
using Application.Workflows.ReAct.Dto;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;

namespace Application.Workflows.Users;

public class UserNode(IAgent agent) : ReflectingExecutor<UserNode>(WorkflowConstants.UserNode), 
    IMessageHandler<ActUserRequest>, 
    IMessageHandler<UserResponse>
{
    public async ValueTask HandleAsync(ActUserRequest actUserRequest, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        var sessionState = await context.SessionState();

        await foreach (var update in agent.RunStreamingAsync(new ChatMessage(ChatRole.User, actUserRequest.Message), cancellationToken: cancellationToken))
        {
            await context.AddEventAsync(new ConversationStreamingEvent(update.Text, false, sessionState.RequestId), cancellationToken);
        }

        await context.AddEventAsync(new ConversationStreamingEvent(string.Empty, true, sessionState.RequestId), cancellationToken);

        await context.SendMessageAsync(new UserRequest(actUserRequest.Message), cancellationToken: cancellationToken);
    }

    public async ValueTask HandleAsync(UserResponse message, IWorkflowContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await context.RequestId(message.RequestId);
        
        await context.SendMessageAsync(new ActObservation(message.Message), cancellationToken: cancellationToken);
    }
}