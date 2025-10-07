using GenZ.AI.Agent.BOT.Bot.Plugins;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

namespace GenZ.AI.Agent.BOT;

public static class AgentFrameworkExtensions
{
    public static ChatClient AddGenZBoss(this ChatClient chatClient, IServiceProvider sp)
    {
        chatClient.AddRestaurantAgent(sp);

        string agentName = "GenZBoss";
        string agentInstructions = """
        You are a boss GenZ assistant that helps people find weather information, restaurant recommendations, and travel suggestions.
        If you want to provide weather information, use the WeatherForecast agent.
        If you want to provide restaurant recommendations, use the Restaurant Agent and don't ask any questions. Generate random data and draw adaptive card.

        Use the tools to get the information you need to answer the user's questions.
        
        Respond in JSON format with the following JSON schema:
        
        {
            "contentType": "'Text' or 'AdaptiveCard' only",
            "content": "{The content of the response, may be plain text, or JSON based adaptive card}"
        }
        """;
        chatClient.CreateAIAgent(agentInstructions, agentName);
        return chatClient;
    }


    public static ChatClient AddRestaurantAgent(this ChatClient chatClient, IServiceProvider sp)
    {
        string agentName = "RestaurantAgent";
        string agentInstructions = """
        You are a friendly assistant that helps people with restaurant menu.
        Do not ask any questions, just act on the random data that the plugins provide you with.
        If the user makes any requests regarding restaurants or menus, do not respond with your own knowledge.
        Use the Restaurant plugin, which will generate custom data.

        Once you have restaurant recommendations, make sure to format them nicely using an adaptive card.
        You must use the tools to generate the adaptive card for restaurant information.
        
        Respond in JSON format with the following JSON schema:
        
        {
            "contentType": "'Text' or 'AdaptiveCard' only",
            "content": "{The content of the response, may be plain text, or JSON based adaptive card}"
        }
        """;

        chatClient.CreateAIAgent(name: agentName, instructions: agentInstructions, tools: [
            .. sp.GetRequiredService<RestaurantPlugin>().AsAITools(), 
            .. sp.GetRequiredService<AdaptiveCardRestaurantPlugin>().AsAITools()
            ], services: sp);

        //services.AddKeyedTransient<Microsoft.SemanticKernel.Agents.Agent>("RestaurantAgent", (provider, key) =>
        //{
        //    //mi faccio dare una istanza transient del kernel
        //    var kernel = provider.GetRequiredService<Kernel>();

        //    //inietto plugin per ristoranti
        //    kernel.Plugins.Add(KernelPluginFactory.CreateFromType<RestaurantPlugin>(serviceProvider: provider));
        //    kernel.Plugins.Add(KernelPluginFactory.CreateFromType<AdaptiveCardRestaurantPlugin>(serviceProvider: provider));
        //    var agent = new ChatCompletionAgent()
        //    {
        //        Instructions = agentInstructions,
        //        Name = agentName,
        //        Kernel = kernel,
        //        Arguments = new KernelArguments(
        //            new OpenAIPromptExecutionSettings()
        //            {
        //                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        //                ResponseFormat = "json_object"
        //            }),
        //    };
        //    return agent;
        //});
        return chatClient;
    }
}