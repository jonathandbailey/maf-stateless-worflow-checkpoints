using Microsoft.Extensions.Options;

namespace Application.Infrastructure;

public class ArtifactRepository(IAzureStorageRepository repository, IOptions<AzureStorageSeedSettings> settings) : IArtifactRepository
{
    private const string ApplicationJsonContentType = "application/json";

    public async Task SaveAsync(Guid sessionId, string artifact, string name)
    {
        await repository.UploadTextBlobAsync(GetCheckpointFileName(sessionId, name),
            settings.Value.ContainerName,
            artifact, ApplicationJsonContentType);
    }

    private static string GetCheckpointFileName(Guid sessionId, string name)
    {
        return $"artifacts/{sessionId}/{name}.json";
    }
}

public interface IArtifactRepository
{
    Task SaveAsync(Guid sessionId, string artifact, string name);
}