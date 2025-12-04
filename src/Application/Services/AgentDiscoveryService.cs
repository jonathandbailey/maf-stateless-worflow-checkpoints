using System.Text.Json;

namespace Application.Services;

public class AgentDiscoveryService : IAgentDiscoveryService
{
    private readonly List<AgentCapability> _capabilities =
    [
        new AgentCapability("research_flights", "Searches flight options between two cities or airports.",
            ["origin", "destination", "depart_date", "return_date"]),

        new AgentCapability("research_trains", "Searches train options between two cities or train stations.",
            ["origin", "destination", "depart_date", "return_date"]),

        new AgentCapability("research_hotels", "Searches hotel options at the destination.",
            ["destination", "depart_date", "return_date"])
    ];

    public string Get()
    {
        return JsonSerializer.Serialize(_capabilities);
    }
}

public interface IAgentDiscoveryService
{
    string Get();
}

public record AgentCapability(string Name, string Description, List<string> Inputs);