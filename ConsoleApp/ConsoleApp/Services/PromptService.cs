namespace ConsoleApp.Services;

public interface IPromptService
{
    string GetPrompt(string key);
    bool TryGetPrompt(string key, out string? prompt);
}

public class PromptService : IPromptService
{
    private readonly Dictionary<string, string> _prompts = new(StringComparer.OrdinalIgnoreCase);

    public PromptService()
    {
        LoadPrompts();
    }

    public string GetPrompt(string key)
    {
        if (_prompts.TryGetValue(key, out var prompt))
        {
            return prompt;
        }

        throw new KeyNotFoundException($"Prompt with key '{key}' not found.");
    }

    public bool TryGetPrompt(string key, out string? prompt)
    {
        return _prompts.TryGetValue(key, out prompt);
    }

    private void LoadPrompts()
    {
        var promptsDirectory = Path.Combine(AppContext.BaseDirectory, "Prompts");

        if (!Directory.Exists(promptsDirectory))
        {
            return;
        }

        var promptFiles = Directory.GetFiles(promptsDirectory, "*.md", SearchOption.AllDirectories);

        foreach (var file in promptFiles)
        {
            var key = Path.GetFileNameWithoutExtension(file);
            var content = File.ReadAllText(file).Trim();
            _prompts[key] = content;
        }
    }
}