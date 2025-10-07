using GenZ.AI.Agent.BOT.Bot.Models;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace GenZ.AI.Agent.BOT.Bot.Plugins;

public class AdaptiveCardRestaurantPlugin
{
    private readonly ChatClient chatClient;

    public AdaptiveCardRestaurantPlugin(ChatClient chatClient)
    {
        this.chatClient = chatClient;
    }

    [Description("Create an adaptive card from restaurant data")]

    public async Task<string> GetAdaptiveCardForData(RestaurantModel data)
    {
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var promptContent = await File.ReadAllTextAsync(Path.Combine(dir, @"Bot\Plugins\Templates\GenerateRestaurantCardTemplate.yaml"));
        var restaurantAdaptiveContent = await File.ReadAllTextAsync(Path.Combine(dir, @"Bot\Cards\Restaurant.json"));


        var agent = chatClient.CreateAIAgent(
            name: "AdaptiveCardRestaurantPlugin",
            instructions: "You are a function that creates an adaptive card from restaurant data. Use the provided prompt template to generate the adaptive card.",
            tools: [AIFunctionFactory.CreateDeclaration],
            services: null
        );
        agent.RunAsync()


        // Create a prompt from the YAML file
        var template = KernelFunctionYaml.ToPromptTemplateConfig(promptContent);

        Dictionary<string, object> _parameters = [];
        _parameters["adaptiveCardSample"] = restaurantAdaptiveContent;
        _parameters["restaurantData"] = JsonSerializer.Serialize(data);
        KernelArguments promptArgs = new(_parameters, template.ExecutionSettings);

        var prompt = kernel.CreateFunctionFromPrompt(template);
        // Invoke the model to get a response
        var response = await prompt.InvokeAsync(kernel, promptArgs);

        return response.ToString();        
    }

    public IEnumerable<AITool> AsAITools()
    {
        yield return AIFunctionFactory.Create(this.GetAdaptiveCardForData);
    }
}
