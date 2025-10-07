using Azure.AI.OpenAI;
using Azure.Identity;
using GenZ.AI.Agent.BOT;
using GenZ.AI.Agent.BOT.Bot;
using GenZ.AI.Agent.BOT.Bot.Agents;
using GenZ.AI.Agent.BOT.Bot.Plugins;
using Microsoft.Agents.AI;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Storage;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();
builder.Logging.AddConsole();

// Register the AI service of your choice. AzureOpenAI and OpenAI are demonstrated...
var config = builder.Configuration.Get<ConfigOptions>();

builder.Services
    .AddScoped<WeatherForecastPlugin>()
    .AddScoped<DateTimePlugin>()
    .AddScoped<AdaptiveCardWeatherPlugin>()
    .AddScoped<RestaurantPlugin>()
    .AddScoped<AdaptiveCardRestaurantPlugin>();

builder.Services.AddTransient((sp) => 
        new AzureOpenAIClient(new(config.Azure.OpenAIEndpoint), new ApiKeyCredential(config.Azure.OpenAIApiKey))
            .GetChatClient(config.Azure.OpenAIDeploymentName)
            .AddGenZBoss(sp)
        );

// Register the WeatherForecastAgent
builder.Services.AddTransient<OrchestrationAgent>();

// Add AspNet token validation
builder.Services.AddBotAspNetAuthentication(builder.Configuration);

// Register IStorage.  For development, MemoryStorage is suitable.
// For production Agents, persisted storage should be used so
// that state survives Agent restarts, and operate correctly
// in a cluster of Agent instances.
builder.Services.AddSingleton<IStorage, MemoryStorage>();

// Add AgentApplicationOptions from config.
builder.AddAgentApplicationOptions();

// Add AgentApplicationOptions.  This will use DI'd services and IConfiguration for construction.
builder.Services.AddTransient<AgentApplicationOptions>();

// Add the bot (which is transient)
builder.AddAgent<GenZ.AI.Agent.BOT.Bot.GenZBotApplication>();
builder.Services.AddScoped<GenZHandleChatRequest>();
builder.Services.AddGenZBoss();

 var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/messages", async (HttpRequest request, HttpResponse response, IAgentHttpAdapter adapter, IAgent agent, CancellationToken cancellationToken) =>
{
    await adapter.ProcessAsync(request, response, agent, cancellationToken);
});

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Playground")
{
    app.MapGet("/", () => "Weather Bot");
    app.UseDeveloperExceptionPage();
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}

app.Run();

