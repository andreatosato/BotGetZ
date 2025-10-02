namespace GenZ.AI.Agent.BOT.Bot.Models;

public class WeatherForecast
{
    /// <summary>
    /// A date for the weather forecast
    /// </summary>
    public string Date { get; set; }

    /// <summary>
    /// Max temperature
    /// </summary>
    public int MaxTemperature { get; set; }

    /// <summary>
    /// Gets or sets the minimum allowable temperature value.
    /// </summary>
    public int MinTemperature { get; set; }
}
