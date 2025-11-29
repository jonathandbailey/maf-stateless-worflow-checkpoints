namespace Application.Workflows.ReAct.Dto;

public sealed record ConversationRequest(Guid SessionId, Guid UserId, string Message);

public sealed record ConversationResponse(Guid SessionId, Guid UserId, string Message);