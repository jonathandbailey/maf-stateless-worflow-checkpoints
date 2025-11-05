namespace Application.Workflows.Conversations.Dto;

public class WorkflowResponse(WorkflowResponseState state)
{
    public WorkflowResponseState State { get; init; } = state;
}

public enum WorkflowResponseState
{
    UserInputRequired,
    Completed
}