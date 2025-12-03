using Application.Workflows.ReAct.Dto;
using Microsoft.Agents.AI.Workflows;

namespace Application.Workflows;

public static class WorkflowExtensions
{

    public static async Task<SessionState> SessionState(this IWorkflowContext context)
    {
       return await context.ReadStateAsync<SessionState>(WorkflowConstants.SessionState, scopeName: WorkflowConstants.ScopeGlobal);
    }

    public static async Task SessionState(this IWorkflowContext context, SessionState state)
    {
        await context.QueueStateUpdateAsync(WorkflowConstants.SessionState, state, scopeName: WorkflowConstants.ScopeGlobal);
    }

    public static async Task RequestId(this IWorkflowContext context, Guid requestId)
    {
        var sessionState = await context.SessionState();

        sessionState = sessionState with { RequestId = requestId };

        await context.SessionState(sessionState);
    }

    public static async Task ExchangeId(this IWorkflowContext context, Guid exchangeId)
    {
        await context.QueueStateUpdateAsync(WorkflowConstants.ExchangeId, exchangeId, scopeName: WorkflowConstants.ScopeGlobal);
    }

    public static async Task<Checkpointed<StreamingRun>> CreateStreamingRun<T>(this Workflow workflow, T message, WorkflowState state, CheckpointManager checkpointManager, CheckpointInfo? checkpointInfo) where T : notnull
    {
        switch (state)
        {
            case WorkflowState.Initialized:
                return await StartStreamingRun(workflow, message, checkpointManager);
            case WorkflowState.WaitingForUserInput:
                return await ResumeStreamingRun(workflow, checkpointInfo, checkpointManager);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task<Checkpointed<StreamingRun>> StartStreamingRun<T>(Workflow workflow, T message, CheckpointManager checkpointManager) where T : notnull
    {
        return await InProcessExecution.StreamAsync(workflow, message, checkpointManager);
    }

    private static async Task<Checkpointed<StreamingRun>> ResumeStreamingRun(Workflow workflow,
        CheckpointInfo? checkpointInfo, CheckpointManager checkpointManager)
    {
        if (checkpointInfo == null)
            throw new ArgumentNullException(nameof(checkpointInfo),"Are CheckpointInfo is required to resume the workflow.");
        
        var run = await InProcessExecution.ResumeStreamAsync(workflow, checkpointInfo, checkpointManager,
            checkpointInfo.RunId);
    
        return run;

    }

    public static WorkflowResponse HandleRequestForUserInput(this RequestInfoEvent requestInfoEvent)
    {
        var data = requestInfoEvent.Data as ExternalRequest;

        if (data?.Data == null)
        {
            return new WorkflowResponse(WorkflowState.Error,
                "Invalid request event: missing data");
        }

        if (data.Data.AsType(typeof(UserRequest)) is not UserRequest userRequest)
        {
            return new WorkflowResponse(WorkflowState.Error,
                "Invalid request event: unable to parse UserRequest");
        }

        if (string.IsNullOrWhiteSpace(userRequest.Message))
        {
            return new WorkflowResponse(WorkflowState.Error,
                "Invalid request event: UserRequest message is empty");
        }

        return new WorkflowResponse(WorkflowState.WaitingForUserInput, userRequest.Message);
    }

}