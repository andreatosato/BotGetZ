using GenZ.AI.Agent.BOT.Bot.Agents;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;

namespace GenZ.AI.Agent.BOT.Bot;

public class GenZBotApplication : AgentApplication
{
    private WeatherForecastAgent _weatherAgent;
    private GenZHandleChatRequest _coordinatorAgent;

    public GenZBotApplication(AgentApplicationOptions options, GenZHandleChatRequest coordinatorAgent) : base(options)
    {
        _coordinatorAgent = coordinatorAgent ?? throw new ArgumentNullException(nameof(coordinatorAgent));

        OnConversationUpdate(ConversationUpdateEvents.MembersAdded, WelcomeMessageAsync);
        OnActivity(ActivityTypes.Message, MessageActivityAsync, rank: RouteRank.Last);
    }

    protected async Task MessageActivityAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        await _coordinatorAgent.HandleChatRequest(turnContext, turnState, cancellationToken);
    }

    protected async Task WelcomeMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
        {
            if (member.Id != turnContext.Activity.Recipient.Id)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Hello and Welcome! I'm here to help with all your weather forecast needs!"), cancellationToken);
            }
        }
    }
}