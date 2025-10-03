using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection.Extensions;
using GenZ.AI.Agent.BOT.Bot.Agents;
using Microsoft.Extensions.DependencyInjection;

namespace GenZ.AI.Agent.BOT.Bot;

public class GenZHandleChatRequest
{
    private OrchestrationAgent _orchestrationAgent;
    private Kernel _kernel;
    private readonly IServiceProvider _serviceProvider;

    public GenZHandleChatRequest(Kernel kernel, IServiceProvider serviceProvider)
    {
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _serviceProvider = serviceProvider;
    }

    public async Task HandleChatRequest(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
    {
        // Setup local service connection
        //ServiceCollection serviceCollection = [
        //    new ServiceDescriptor(typeof(ITurnState), turnState),
        //    new ServiceDescriptor(typeof(ITurnContext), turnContext),
        //    new ServiceDescriptor(typeof(Kernel), _kernel),
        //];

        // Start a Streaming Process 
        await turnContext.StreamingResponse.QueueInformativeUpdateAsync("Working on a response for you");

        ChatHistory chatHistory = turnState.GetValue("conversation.chatHistory", () => new ChatHistory());
        var genzBoss = _serviceProvider.GetRequiredKeyedService<Microsoft.SemanticKernel.Agents.Agent>("GenZBoss");
        _orchestrationAgent = new OrchestrationAgent(genzBoss);

        // Invoke the WeatherForecastAgent to process the message
        AgentResponse forecastResponse = await _orchestrationAgent.InvokeAgentAsync(turnContext.Activity.Text, chatHistory);
        if (forecastResponse == null)
        {
            turnContext.StreamingResponse.QueueTextChunk("Sorry, I couldn't get the weather forecast at the moment.");
            await turnContext.StreamingResponse.EndStreamAsync(cancellationToken);
            return;
        }

        // Create a response message based on the response content type from the WeatherForecastAgent
        // Send the response message back to the user. 
        switch (forecastResponse.ContentType)
        {
            case AgentResponseContentType.Text:
                turnContext.StreamingResponse.QueueTextChunk(forecastResponse.Content);
                break;
            case AgentResponseContentType.AdaptiveCard:
                turnContext.StreamingResponse.FinalMessage = MessageFactory.Attachment(new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = forecastResponse.Content,
                });
                break;
            default:
                break;
        }
        await turnContext.StreamingResponse.EndStreamAsync(cancellationToken); // End the streaming response
    }
}
