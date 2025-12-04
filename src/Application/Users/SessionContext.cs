namespace Application.Users;

public class SessionContext(Guid userId, Guid sessionId) : ISessionContext
{
    public Guid UserId { get; } = userId;

    public Guid SessionId { get;  } = sessionId;
}

public interface ISessionContext
{
    Guid UserId { get;  }
    Guid SessionId { get;  }
}