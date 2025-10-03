using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using System.Text.Json.Nodes;
using GenZ.AI.Agent.BOT.Bot.Plugins;

namespace GenZ.AI.Agent.BOT.Bot.Agents;

public class OrchestrationAgent
{
    private readonly Microsoft.SemanticKernel.Agents.Agent genzAgent;

    public OrchestrationAgent([FromKeyedServices("GenZBoss")] Microsoft.SemanticKernel.Agents.Agent genzAgent)
    {
        this.genzAgent = genzAgent;
    }
    /// <summary>
    /// Invokes the agent with the given input and returns the response.
    /// </summary>
    /// <param name="input">A message to process.</param>
    /// <returns>An instance of <see cref="AgentResponse"/></returns>
    public async Task<AgentResponse> InvokeAgentAsync(string input, ChatHistory chatHistory)
    {
        ArgumentNullException.ThrowIfNull(chatHistory);
        AgentThread thread = new ChatHistoryAgentThread();
        ChatMessageContent message = new(AuthorRole.User, input);
        chatHistory.Add(message);

        StringBuilder sb = new();
        await foreach (ChatMessageContent response in this.genzAgent.InvokeAsync(chatHistory, thread: thread))
        {
            chatHistory.Add(response);
            sb.Append(response.Content);
        }

        // Make sure the response is in the correct format and retry if necessary
        try
        {
            string resultContent = sb.ToString();
            var jsonNode = JsonNode.Parse(resultContent);
            AgentResponse result = new AgentResponse()
            {
                Content = jsonNode["content"].ToString(),
                ContentType = Enum.Parse<AgentResponseContentType>(jsonNode["contentType"].ToString(), true)
            };
            return result;
        }
        catch (Exception je)
        {
            return await InvokeAgentAsync($"That response did not match the expected format. Please try again. Error: {je.Message}", chatHistory);
        }
    }
}
