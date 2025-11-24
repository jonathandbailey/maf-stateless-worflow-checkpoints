using Application.Agents;
using Application.Infrastructure;
using Application.Workflows.ReAct.Dto;
using Application.Workflows.ReAct.Nodes;
using Application.Workflows.ReWoo.Dto;
using Application.Workflows.ReWoo.Nodes;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace Application.Workflows;

public class WorkflowFactory(IAgentFactory agentFactory, IArtifactRepository artifactRepository) : IWorkflowFactory
{
    public async Task<Workflow<ChatMessage>> Create()
    {
        var reasonAgent = await agentFactory.CreateReasonAgent();

        var actAgent = await agentFactory.CreateActAgent();

        var orchestrationAgent = await agentFactory.CreateOrchestrationAgent();

        var flightAgent = await agentFactory.CreateFlightWorkerAgent();

        var hotelAgent = await agentFactory.CreateHotelWorkerAgent();

        var trainAgent = await agentFactory.CreateTrainWorkerAgent();

        var requestPort = RequestPort.Create<UserRequest, UserResponse>("user-input");

        var reasonNode = new ReasonNode(reasonAgent);
        var actNode = new ActNode(actAgent);
        var orchestrationNode = new OrchestrationNode(orchestrationAgent);

        var flightWorkerNode = new FlightWorkerNode(flightAgent);
        var hotelWorkerNode = new HotelWorkerNode(hotelAgent);
        var trainWorkerNode = new TrainWorkerNode(trainAgent);

        var artifactStorageNode = new ArtifactStorageNode(artifactRepository);

        var builder = new WorkflowBuilder(reasonNode);

        builder.AddEdge(reasonNode, actNode);
        builder.AddEdge(actNode, requestPort);
        builder.AddEdge(requestPort, actNode);
        builder.AddEdge(actNode, reasonNode);
        builder.AddEdge(actNode, orchestrationNode);

        builder.AddEdge<OrchestratorWorkerTaskDto>(
            source: orchestrationNode,
            target: flightWorkerNode,
            condition: result => result?.Worker == "research_flights");

        builder.AddEdge<OrchestratorWorkerTaskDto>(
            source: orchestrationNode,
            target: trainWorkerNode,
            condition: result => result?.Worker == "research_trains");

        builder.AddEdge<OrchestratorWorkerTaskDto>(
            source: orchestrationNode,
            target: hotelWorkerNode,
            condition: result => result?.Worker == "research_hotels");

        builder.AddEdge(flightWorkerNode, artifactStorageNode);
        builder.AddEdge(hotelWorkerNode, artifactStorageNode);
        builder.AddEdge(trainWorkerNode, artifactStorageNode);

        return await builder.BuildAsync<ChatMessage>();
    }
}

public interface IWorkflowFactory
{
    Task<Workflow<ChatMessage>> Create();
}