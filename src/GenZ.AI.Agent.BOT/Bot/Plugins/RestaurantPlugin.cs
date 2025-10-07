using GenZ.AI.Agent.BOT.Bot.Models;
using Microsoft.Extensions.AI;
using System.ComponentModel;

namespace GenZ.AI.Agent.BOT.Bot.Plugins;

public class RestaurantPlugin
{
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

    public IEnumerable<AITool> AsAITools()
    {
        yield return AIFunctionFactory.Create(this.GetRestaurantMenu);
    }
}

