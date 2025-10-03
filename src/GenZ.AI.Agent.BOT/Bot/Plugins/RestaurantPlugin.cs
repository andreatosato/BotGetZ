using GenZ.AI.Agent.BOT.Bot.Models;
using Microsoft.Agents.Builder;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace GenZ.AI.Agent.BOT.Bot.Plugins;

public class RestaurantPlugin(ITurnContext turnContext)
{
    [KernelFunction]
    [Description("Get a sample restaurant menu")]
    public RestaurantModel GetRestaurantMenu()
    {
        return new RestaurantModel
        {
            Title = "Gourmet Paradise",
            Subtitle = "Exquisite Dining Experience",
            Food = MenuItemGenerator.GenerateFakeMenu()
        };
    }
}

