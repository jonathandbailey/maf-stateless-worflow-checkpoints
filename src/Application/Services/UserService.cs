using Application.Dto;
using Application.Infrastructure;
using Application.Settings;
using Microsoft.Extensions.Options;

namespace Application.Services;


public class UserService(
    IAzureStorageRepository storageRepository, 
    IOptions<AzureStorageSeedSettings> settings, 
    IOptions<List<UserSettings>> userSettings) : IUserService
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