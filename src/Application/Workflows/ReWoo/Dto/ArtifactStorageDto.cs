namespace Application.Workflows.ReWoo.Dto;

public class ArtifactStorageDto(string key, string content)
{
    public string Key { get; } = key;

    public string Content { get; } = content;
}