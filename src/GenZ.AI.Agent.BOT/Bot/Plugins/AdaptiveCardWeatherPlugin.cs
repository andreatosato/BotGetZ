using GenZ.AI.Agent.BOT.Bot.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GenZ.AI.Agent.BOT.Bot.Plugins;

public class AdaptiveCardWeatherPlugin
{
    [KernelFunction("create_weather_adaptive_card")]
    [Description("Create an adaptive card from weather data")]

    public async Task<string> GetAdaptiveCardForData(Kernel kernel, string data)
    {
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var promptContent = await File.ReadAllTextAsync(Path.Combine(dir, @"Bot\Plugins\Templates\GenerateWeatherCardTemplate.yaml"));
        var weatherAdaptiveContent = await File.ReadAllTextAsync(Path.Combine(dir, @"Bot\Cards\Weather.json"));
       
        // Create a prompt from the YAML file
        var template = KernelFunctionYaml.ToPromptTemplateConfig(promptContent);

        Dictionary<string, object> _parameters = [];
        _parameters["adaptiveCardSample"] = weatherAdaptiveContent;
        _parameters["weatherData"] = data;
        KernelArguments promptArgs = new(_parameters, template.ExecutionSettings);

        var prompt = kernel.CreateFunctionFromPrompt(template);
        // Invoke the model to get a response
        var response = await prompt.InvokeAsync(kernel, promptArgs);
        return response.ToString();        
    }
}
