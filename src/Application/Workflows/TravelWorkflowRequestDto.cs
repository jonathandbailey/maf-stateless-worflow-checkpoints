using Microsoft.Extensions.AI;

namespace Application.Workflows;

public class TravelWorkflowRequestDto(ChatMessage message, Guid userId, Guid sessionId)
{
    public Guid SessionId { get; } = sessionId;

    public Guid UserId { get; } = userId;

    public ChatMessage Message { get; } = message;
}