using Microsoft.Agents.AI.Workflows;

namespace Application.Workflows.Conversations.Dto;

public class WorkflowStateDto(ConversationCheckpointStore store, WorkflowState state, CheckpointInfo checkpointInfo)
{
    public ConversationCheckpointStore Store { get; } = store;

    public WorkflowState State { get; } = state;

    public CheckpointInfo CheckpointInfo { get; } = checkpointInfo;
}