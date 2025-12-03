namespace Api.Dto;

public record ConversationRequestDto(string Message, Guid SessionId, Guid ExchangeId);

public record ConversationResponseDto(string Message, Guid SessionId, Guid ExchangeId);