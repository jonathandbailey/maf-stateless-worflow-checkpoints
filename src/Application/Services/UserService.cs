using Application.Dto;

namespace Application.Services;


public class UserService : IUserService
{
    public async Task<UserProfileDto> AuthenticateUser(string username)
    {
        throw new NotImplementedException();
    }
}

public interface IUserService
{
    Task<UserProfileDto> AuthenticateUser(string username);
}