using Bogus;

namespace GenZ.AI.Agent.BOT.Bot.Models;

public class RestaurantModel
{
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public List<MenuItem> Food { get; set; }
}

public class MenuItem
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Tags { get; set; }
    public string OldPrice { get; set; }
    public string Price { get; set; }
    public string Image { get; set; }
}


public static class MenuItemGenerator
{
    public static List<MenuItem> GenerateFakeMenu()
    {
        var faker = new Faker<MenuItem>()
            .RuleFor(m => m.Name, f => f.Commerce.ProductName())
            .RuleFor(m => m.Description, f => f.Lorem.Sentence())
            .RuleFor(m => m.Tags, f => f.Random.Bool() ? "• Vegan • Local made - " + f.Company.CompanyName() : "• Organic • Farm to table - " + f.Company.CompanyName())
            .RuleFor(m => m.OldPrice, f => "$" + f.Commerce.Price(10, 90))
            .RuleFor(m => m.Price, (f, m) => {
                var old = decimal.Parse(m.OldPrice.Replace("$", ""));
                var discount = f.Random.Decimal(1, 3);
                return "$" + (old - discount).ToString("0.00");
            })
            .RuleFor(m => m.Image, f => f.Image.PicsumUrl(width: 100, height: 150));

        return faker.Generate(Random.Shared.Next(1,6));
    }
}