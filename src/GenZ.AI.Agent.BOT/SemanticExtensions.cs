using GenZ.AI.Agent.BOT.Bot.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace GenZ.AI.Agent.BOT;

public static class SemanticExtensions
{
    public static IServiceCollection AddGenZBoss(this IServiceCollection services)
    {
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

        //mi faccio dare una istanza transient del kernel
        services.AddWeatherAgent();
        services.AddRestaurantAgent();

        services.AddKeyedScoped<Microsoft.SemanticKernel.Agents.Agent>("GenZBoss", (provider, key) =>
        {
            var kernel = provider.GetRequiredService<Kernel>();
            var weatherAgent = kernel.Services.GetRequiredKeyedService<Microsoft.SemanticKernel.Agents.Agent>("WeatherAgent");
            kernel.Plugins.AddFromFunctions(
                "WeatherAgent",
                [AgentKernelFunctionFactory.CreateFromAgent(weatherAgent)]
            );

            var restaurantAgent = kernel.Services.GetRequiredKeyedService<Microsoft.SemanticKernel.Agents.Agent>("RestaurantAgent");
            kernel.Plugins.AddFromFunctions(
                "RestaurantAgent",
                [AgentKernelFunctionFactory.CreateFromAgent(restaurantAgent)]
            );

            Dictionary<string, object> parameters = new();
            var genZBoss = new ChatCompletionAgent()
            {
                Instructions = agentInstructions,
                Name = agentName,
                Kernel = kernel,
                Arguments = new KernelArguments(
                    parameters,
                    new Dictionary<string, PromptExecutionSettings>
                    {
                        {
                            PromptExecutionSettings.DefaultServiceId,
                            new OpenAIPromptExecutionSettings()
                            {
                                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                                ResponseFormat = "json_object"
                            }
                        }
                    }
                ),
            };
            return genZBoss;
        });

        return services;
    }

    public static IServiceCollection AddWeatherAgent(this IServiceCollection services)
    {
        string agentName = "WeatherForecastAgent";
        string agentInstructions = """
        You are a friendly assistant that helps people find a weather forecast for a given time and place.
        You may ask follow up questions until you have enough information to answer the customers question,
        but once you have a forecast forecast, make sure to format it nicely using an adaptive card.
        You must use the tools to generate the adaptive card.
        
        Respond in JSON format with the following JSON schema:
        
        {
            "contentType": "'Text' or 'AdaptiveCard' only",
            "content": "{The content of the response, may be plain text, or JSON based adaptive card}"
        }
        """;

    services.AddKeyedTransient<Microsoft.SemanticKernel.Agents.Agent>("WeatherAgent", (provider, key) =>
        {
            //mi faccio dare una istanza transient del kernel
            var kernel = provider.GetRequiredService<Kernel>();

            //inietto plugin per mercurio
            kernel.Plugins.Add(KernelPluginFactory.CreateFromType<DateTimePlugin>(serviceProvider: provider));
            kernel.Plugins.Add(KernelPluginFactory.CreateFromType<WeatherForecastPlugin>(serviceProvider: provider));
            kernel.Plugins.Add(KernelPluginFactory.CreateFromType<AdaptiveCardWeatherPlugin>(serviceProvider: provider));

            Dictionary<string, object> parameters = [];
            parameters["today"] = DateTime.UtcNow.GetDateTimeFormats('D')[0];
            var args = new KernelArguments(new OpenAIPromptExecutionSettings()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ResponseFormat = "json_object"
            });
            var agent = new ChatCompletionAgent()
            {
                Instructions = agentInstructions,
                Name = agentName,
                Kernel = kernel,
                Arguments = new KernelArguments(
                    parameters,
                    new Dictionary<string, PromptExecutionSettings>
                    {
                        {
                            PromptExecutionSettings.DefaultServiceId,
                            new OpenAIPromptExecutionSettings()
                            {
                                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                                ResponseFormat = "json_object"
                            }
                        }
                    }
                ),
            };
            return agent;
        });
        return services;
    }

    public static IServiceCollection AddRestaurantAgent(this IServiceCollection services)   
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

        services.AddKeyedTransient<Microsoft.SemanticKernel.Agents.Agent>("RestaurantAgent", (provider, key) =>
        {
            //mi faccio dare una istanza transient del kernel
            var kernel = provider.GetRequiredService<Kernel>();

            //inietto plugin per ristoranti
            kernel.Plugins.Add(KernelPluginFactory.CreateFromType<RestaurantPlugin>(serviceProvider: provider));
            kernel.Plugins.Add(KernelPluginFactory.CreateFromType<AdaptiveCardRestaurantPlugin>(serviceProvider: provider));
            var agent = new ChatCompletionAgent()
            {
                Instructions = agentInstructions,
                Name = agentName,
                Kernel = kernel,
                Arguments = new KernelArguments(
                    new OpenAIPromptExecutionSettings()
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                    ResponseFormat = "json_object"
                }),
            };
            return agent;
        });
        return services;
    }
}
