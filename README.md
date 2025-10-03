# 365-Agents-Gen-Z

A GenZ-themed AI assistant bot built with Microsoft 365 Agents SDK and Semantic Kernel that provides weather information, restaurant recommendations, and travel suggestions through natural language conversations.

## Overview

This project showcases an intelligent bot application that leverages Azure OpenAI and Semantic Kernel's agent framework to create a multi-agent system. The GenZ Boss Agent coordinates between specialized sub-agents to answer user queries:

- **Weather Agent**: Provides weather forecasts for specific locations and times
- **Restaurant Agent**: Offers restaurant recommendations with adaptive cards
- **Travel Suggestions**: Helps with travel planning (in development)

The application uses an orchestration pattern where a main "GenZ Boss" agent delegates tasks to specialized agents, demonstrating the power of multi-agent AI systems.

## Architecture

The project consists of two main components:

1. **GenZ.AI.Agent.BOT**: The main bot application using Microsoft Agents SDK and Semantic Kernel
   - Multi-agent orchestration with GenZ Boss coordinator
   - Weather forecast agent with adaptive card rendering
   - Restaurant recommendation agent with Bogus data generation
   - Built on ASP.NET Core 8.0

2. **M365Agent**: Microsoft 365 Teams integration template
   - Teams and Microsoft 365 Agents Playground support
   - Azure deployment configuration

## Features

- ✨ Natural language interaction with AI agents
- 🌤️ Weather forecasts with beautiful adaptive cards
- 🍽️ Restaurant recommendations
- 🎯 Multi-agent orchestration for complex queries
- 📱 Microsoft Teams integration
- 🔄 Streaming responses for better user experience
- 🎨 Adaptive Cards for rich content display

## Prerequisites

To run this project, you'll need:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Azure OpenAI](https://aka.ms/oai/access) resource with a deployment
- For Teams deployment: [Microsoft 365 Agents Toolkit](https://aka.ms/teams-toolkit)
- An Azure subscription (for deployment)

## Configuration

### Azure OpenAI Setup

1. Create an Azure OpenAI resource in the [Azure Portal](https://portal.azure.com)
2. Deploy a chat completion model (e.g., GPT-4, GPT-3.5-turbo)
3. Note down:
   - API Key
   - Endpoint URL
   - Deployment Name

### Local Development Configuration

Configure your Azure OpenAI settings in one of the following ways:

**Option 1: appsettings.Development.json**
```json
{
  "Azure": {
    "OpenAIApiKey": "<your-azure-openai-api-key>",
    "OpenAIEndpoint": "<your-azure-openai-endpoint>",
    "OpenAIDeploymentName": "<your-azure-openai-deployment-name>"
  }
}
```

**Option 2: User Secrets** (Recommended for development)
```bash
cd src/GenZ.AI.Agent.BOT
dotnet user-secrets set "Azure:OpenAIApiKey" "<your-api-key>"
dotnet user-secrets set "Azure:OpenAIEndpoint" "<your-endpoint>"
dotnet user-secrets set "Azure:OpenAIDeploymentName" "<your-deployment-name>"
```

**Option 3: Environment Variables**
```bash
export Azure__OpenAIApiKey="<your-api-key>"
export Azure__OpenAIEndpoint="<your-endpoint>"
export Azure__OpenAIDeploymentName="<your-deployment-name>"
```

## Getting Started

### Running the GenZ Bot Locally

1. Clone the repository:
```bash
git clone https://github.com/andreatosato/BotGetZ.git
cd BotGetZ
```

2. Configure Azure OpenAI settings (see Configuration section above)

3. Build and run the project:
```bash
cd src/GenZ.AI.Agent.BOT
dotnet restore
dotnet run
```

4. Test with the Microsoft 365 Agents Playground:
   - Set `appsettings.Playground.json` with your Azure OpenAI credentials
   - Run the application with the Playground profile
   - Interact with your bot in the browser

### Running in Microsoft Teams

For Teams integration, use the M365Agent project. See [M365Agent/README.md](src/M365Agent/README.md) for detailed instructions.

Quick steps:
1. Configure Azure OpenAI in `env/.env.local.user`
2. Set up a Dev Tunnel in Visual Studio
3. Sign in with Microsoft 365 account
4. Press F5 to debug in Teams

## Project Structure

```
BotGetZ/
├── src/
│   ├── GenZ.AI.Agent.BOT/          # Main bot application
│   │   ├── Bot/
│   │   │   ├── Agents/             # Agent implementations
│   │   │   ├── Plugins/            # Semantic Kernel plugins
│   │   │   ├── Cards/              # Adaptive card templates
│   │   │   └── Models/             # Data models
│   │   ├── SemanticExtensions.cs   # Agent registration
│   │   ├── Program.cs              # Application entry point
│   │   └── Config.cs               # Configuration models
│   └── M365Agent/                  # Teams integration
│       ├── infra/                  # Azure deployment configs
│       └── README.md               # Teams-specific docs
└── README.md                       # This file
```

## Key Technologies

- **[Microsoft 365 Agents SDK](https://github.com/microsoft/Agents)**: Framework for building conversational agents
- **[Semantic Kernel](https://github.com/microsoft/semantic-kernel)**: AI orchestration library
- **[Azure OpenAI](https://azure.microsoft.com/products/ai-services/openai-service)**: Large language model provider
- **[Adaptive Cards](https://adaptivecards.io/)**: Rich UI card framework
- **[Bogus](https://github.com/bchavez/Bogus)**: Fake data generator for demos
- **ASP.NET Core 8.0**: Web application framework

## Usage Examples

Once running, you can interact with the bot using natural language:

**Weather Queries:**
- "What's the weather in Seattle tomorrow?"
- "Will it rain in New York this weekend?"
- "Show me the forecast for Tokyo"

**Restaurant Recommendations:**
- "Suggest a good restaurant"
- "I'm looking for a place to eat"
- "Recommend a restaurant for dinner"

The bot will use the appropriate agent to handle your request and respond with formatted adaptive cards or text.

## Deployment

### Deploy to Azure

The project includes Bicep templates for Azure deployment:

1. Navigate to `src/M365Agent/infra/`
2. Update `azure.parameters.json` with your settings
3. Deploy using Azure CLI:
```bash
az deployment group create \
  --resource-group <your-resource-group> \
  --template-file azure.bicep \
  --parameters @azure.parameters.json
```

See [M365Agent/README.md](src/M365Agent/README.md) for complete deployment instructions.

## Development

### Adding New Agents

1. Create a new agent class in `Bot/Agents/`
2. Define the agent instructions and behavior
3. Register the agent in `SemanticExtensions.cs`
4. Add necessary plugins in `Bot/Plugins/`

### Adding New Plugins

1. Create a plugin class in `Bot/Plugins/`
2. Annotate methods with `[KernelFunction]`
3. Register the plugin with the appropriate agent's kernel

## Troubleshooting

**Common Issues:**

1. **"Azure OpenAI API Key not configured"**: Ensure your configuration is set correctly in appsettings or user secrets
2. **Authentication errors in Teams**: Verify your Bot ID and Tenant ID in `appsettings.json`
3. **Agent not responding**: Check that your Azure OpenAI deployment is active and has sufficient quota

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is provided as-is for educational and demonstration purposes.

## Additional Resources

- [Microsoft 365 Agents SDK Documentation](https://github.com/microsoft/Agents)
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/)
- [Microsoft Teams Platform](https://learn.microsoft.com/microsoftteams/platform/)
- [Adaptive Cards](https://adaptivecards.io/)

## Support

For issues and questions:
- Create an issue in this repository
- Check the [M365Agent README](src/M365Agent/README.md) for Teams-specific questions
- Refer to the [Microsoft 365 Agents Toolkit documentation](https://aka.ms/teams-toolkit-vs-docs)