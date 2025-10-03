using GenZ.AI.Agent.BOT.Bot.Models;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace GenZ.AI.Agent.BOT.Bot.Plugins;

public class RestaurantPlugin
{
    [KernelFunction]
    [Description("Get a sample restaurant menu with fake data")]
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

