namespace Application.Workflows.ReAct.Dto;

public class ActRequest
{
    public string Thought { get; set; } = string.Empty;

    public string NextAction { get; set; } = string.Empty;

    public Dictionary<string, string> Parameters { get; set; } = new();
}