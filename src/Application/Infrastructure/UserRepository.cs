using Application.Settings;
using Microsoft.Extensions.Options;

namespace Application.Infrastructure;

public class UserRepository(IAzureStorageRepository storageRepository, IOptions<AzureStorageSeedSettings> settings, IOptions<List<UserSettings>> userSettings)
{
    private static string GetFileName(Guid userId)
    {
        return $"{userId}/user-profile.json";
    }
}