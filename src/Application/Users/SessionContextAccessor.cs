namespace Application.Users;

public class SessionContextAccessor : ISessionContextAccessor
{
    private ISessionContext? _context;

    public ISessionContext Context =>
        _context ?? throw new InvalidOperationException("SessionContext not initialized.");

    public void Initialize(Guid userId, Guid sessionId)
    {
        if (_context != null)
            throw new InvalidOperationException("SessionContext already initialized.");

        _context = new SessionContext(userId, sessionId);
    }
}


public interface ISessionContextAccessor
{
    ISessionContext Context { get; }
    void Initialize(Guid userId, Guid sessionId);
}
