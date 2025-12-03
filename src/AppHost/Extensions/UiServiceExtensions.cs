namespace AppHost.Extensions;

public static class UiServiceExtensions
{
    private const string UiName = "ui";
    private const string UiSourcePath = "../../ui";
    private const string UiScriptName = "dev";
    private const string UiViteApiBaseUrl = "VITE_API_BASE_URL";
    private const string UiVitePort = "VITE_PORT";
    private const string UiEndPointReference = "http";

    public static IResourceBuilder<NodeAppResource> AddUiServices(
        this IDistributedApplicationBuilder builder, 
        IResourceBuilder<ProjectResource> api)
    {
        var ui = builder.AddNpmApp(UiName, UiSourcePath, scriptName: UiScriptName)
            .WithReference(api)
            .WaitFor(api)
            .WithEnvironment(UiViteApiBaseUrl, api.GetEndpoint(UiEndPointReference))
            .WithEnvironment("VITE_HUB_BASE_URL", api.GetEndpoint(UiEndPointReference))
            .WithHttpEndpoint(env: UiVitePort)
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile();

        return ui;
    }
}