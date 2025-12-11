namespace Application.Dto;

public record UserProfileDto(Guid UserId, Guid SessionId);

public record UserAuthRequest(string Username);