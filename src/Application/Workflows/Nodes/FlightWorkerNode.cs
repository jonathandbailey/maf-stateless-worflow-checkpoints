using Application.Agents;
using Application.Interfaces;
using Application.Models.Flights;
using Application.Observability;
using Application.Services;
using Application.Workflows.Dto;
using Application.Workflows.Events;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Agents.AI.Workflows.Reflection;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Application.Workflows.Nodes;

public class FlightWorkerNode(IAgent agent, IArtifactRepository artifactRepository, ITravelPlanService travelPlanService) : 
    ReflectingExecutor<FlightWorkerNode>(WorkflowConstants.FlightWorkerNodeName), 
   
    IMessageHandler<CreateFlightOptions>
{
    private const string FlightWorkerNodeError = "Flight Worker Node has failed to execute.";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public async ValueTask HandleAsync(CreateFlightOptions message, IWorkflowContext context,
        CancellationToken cancellationToken = default)
    {
        using var activity = Telemetry.Start($"{WorkflowConstants.FlightWorkerNodeName}{WorkflowConstants.HandleRequest}");

        activity?.SetTag(WorkflowTelemetryTags.Node, WorkflowConstants.FlightWorkerNodeName);

        try
        {
            var serialized = JsonSerializer.Serialize(message);

            WorkflowTelemetryTags.SetInputPreview(activity, serialized);

            var response = await agent.RunAsync(new ChatMessage(ChatRole.User, serialized), cancellationToken: cancellationToken);
   
            WorkflowTelemetryTags.SetInputPreview(activity, response.Text);
       
            var flightOptions = JsonSerializer.Deserialize<FlightActionResultDto>(response.Text, SerializerOptions);

            if (flightOptions == null)
                throw new JsonException("Failed to deserialize flight options in Flight Worker Node");

            var payload = JsonSerializer.Serialize(flightOptions.FlightOptions, SerializerOptions);


            switch (flightOptions.Action)
            {
                case FlightAction.FlightOptionsCreated:
                {
                    var searchArtifact = new ArtifactStorageDto("flights", payload);

                    var travelPlan = await travelPlanService.AddFlightSearchOption(new FlightOptionSearch(searchArtifact.Id));

                    await context.SendMessageAsync(searchArtifact, cancellationToken: cancellationToken);

                    await context.SendMessageAsync(new FlightOptionsCreated(FlightOptionsStatus.Created, UserFlightOptionsStatus.UserChoiceRequired, flightOptions.FlightOptions), cancellationToken: cancellationToken);
                    break;
                }
                case FlightAction.FlightOptionsSelected:
                {
                    var flightOption = flightOptions.FlightOptions.Results.First();

                    var mapped = MapFlightOption(flightOption);

                    await travelPlanService.SelectFlightOption(mapped);
                
                    await context.SendMessageAsync(new FlightOptionsCreated(FlightOptionsStatus.Created, UserFlightOptionsStatus.Selected, flightOptions.FlightOptions), cancellationToken: cancellationToken);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception exception)
        {
            await context.AddEventAsync(new TravelWorkflowErrorEvent(FlightWorkerNodeError, "flights", WorkflowConstants.FlightWorkerNodeName, exception), cancellationToken);
        }
    }

    private FlightOption MapFlightOption(FlightOptionDto flightOption)
    {
        return new FlightOption
        {
            Airline = flightOption.Airline,
            FlightNumber = flightOption.FlightNumber,
            Departure = new FlightEndpoint
            {
                Airport = flightOption.Departure.Airport,
                Datetime = flightOption.Departure.Datetime
            },
            Arrival = new FlightEndpoint
            {
                Airport = flightOption.Arrival.Airport,
                Datetime = flightOption.Arrival.Datetime
            },
            Duration = flightOption.Duration,
            Price = new FlightPrice
            {
                Amount = flightOption.Price.Amount,
                Currency = flightOption.Price.Currency
            }
        };
    }
}