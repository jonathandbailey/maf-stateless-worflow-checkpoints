using Application.Infrastructure;
using Application.Workflows.Conversations;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Workflows.Conversations.Dto;

namespace Application.Workflows;

public class WorkflowManager(IAzureStorageRepository repository, IOptions<AzureStorageSeedSettings> settings) : IWorkflowManager
{
    private const string ApplicationJsonContentType = "application/json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    private ConversationCheckpointStore _checkpointStore;
    private Guid _sessionId;
    public CheckpointManager CheckpointManager { get; private set; }

    public CheckpointInfo CheckpointInfo { get; set; }

    public WorkflowState State { get; private set; } = WorkflowState.Initialized;

    public async Task Initialize(Guid sessionId)
    {
        _sessionId = sessionId;
        
        _checkpointStore = await GetOrCreateCheckpointStore(sessionId);

        CheckpointManager = CheckpointManager.CreateJson(_checkpointStore);
    }

    public void Executing()
    {
        State = WorkflowState.Executing;
    }

    public void WaitingForUserInput()
    {
        State = WorkflowState.WaitingForUserInput;
    }

    public async Task Save()
    {
        var workflowStateDto = new WorkflowStateDto(_checkpointStore, State, CheckpointInfo );
        
        var serializedConversation = JsonSerializer.Serialize(workflowStateDto, SerializerOptions);

        await repository.UploadTextBlobAsync($"{_sessionId}.json", settings.Value.ContainerName,
            serializedConversation, ApplicationJsonContentType);
    }

    private async Task<ConversationCheckpointStore> GetOrCreateCheckpointStore(Guid sessionId)
    {
        var blobExists = await repository.BlobExists($"{sessionId}.json", settings.Value.ContainerName);

        if (blobExists == false)
        {
            State = WorkflowState.Initialized;
            
            return new ConversationCheckpointStore();
        }

        var blob = await repository.DownloadTextBlobAsync($"{sessionId}.json", settings.Value.ContainerName);

        var stateDto = JsonSerializer.Deserialize<WorkflowStateDto>(blob, SerializerOptions);

        if (stateDto == null)
            throw new JsonException($"Failed to deserialize Checkpoint Store for session : {sessionId}");

        State = stateDto.State;

        CheckpointInfo = stateDto.CheckpointInfo;

        return stateDto.Store;
    }
}

public interface IWorkflowManager
{
    CheckpointManager CheckpointManager { get; }
    WorkflowState State { get; }
    CheckpointInfo CheckpointInfo { get; set; }
    Task Save();
    void Executing();
    void WaitingForUserInput();
}