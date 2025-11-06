using Application.Observability;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Checkpointing;
using System.Text.Json;

namespace Application.Workflows.Conversations;

public class ConversationCheckpointStore : JsonCheckpointStore
{
    private readonly Dictionary<CheckpointInfo, JsonElement> _checkpointElements = new();

    public override ValueTask<IEnumerable<CheckpointInfo>> RetrieveIndexAsync(string runId, CheckpointInfo? withParent = null)
    {
        return ValueTask.FromResult<IEnumerable<CheckpointInfo>>(_checkpointElements.Keys.ToList());
    }

    public override ValueTask<CheckpointInfo> CreateCheckpointAsync(string runId, JsonElement value, CheckpointInfo? parent = null)
    {
        using var activity = Telemetry.StarActivity("Checkpointstore-Create");

        var checkpointInfo = new CheckpointInfo(runId, Guid.NewGuid().ToString());

        activity?.SetTag("RunId", checkpointInfo.RunId);
        activity?.SetTag("CheckpointId", checkpointInfo.CheckpointId);

        _checkpointElements.Add(checkpointInfo, value);

        return ValueTask.FromResult(checkpointInfo);
    }

    public override ValueTask<JsonElement> RetrieveCheckpointAsync(string runId, CheckpointInfo key)
    {
        var element = _checkpointElements[key];

        return ValueTask.FromResult(element);
    }
}