
using AI_Agent_Backend.Model;
using Azure;
using OpenAI.Chat;
using System.Text.Json;
using Azure.AI.OpenAI;

namespace AI_Agent_Backend.Service;

internal class AgentService : IAgentService
{
  private readonly ChatClient _chatClient;
  private readonly string _deploymentName;
  private readonly string _endpoint;
  private readonly string _apiKey;
  private readonly AzureOpenAIClient _client;
  private AzureOpenAIClientOptions _options;

  public AgentService(IConfiguration configuration)
  {
    
    _apiKey = configuration["AzureOpenAI:ApiKey"] ?? throw new InvalidOperationException("Missing AzureOpenAI:ApiKey");
    _endpoint = configuration["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Missing AzureOpenAI:Endpoint");
    _deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? throw new InvalidOperationException("Missing AzureOpenAI:DeploymentName");
    _options = new AzureOpenAIClientOptions(AzureOpenAIClientOptions.ServiceVersion.V2025_01_01_Preview);

    _client = new(
      endpoint: new($"{_endpoint}"), 
      credential: new AzureKeyCredential(_apiKey),
      options: _options);

    _chatClient = _client.GetChatClient(_deploymentName);
  }

  async Task<AgentResponse> IAgentService.AskAsync(AgentRequest request)
  {
    var messages = new List<ChatMessage>
    {
      new SystemChatMessage("Sei un assistente tecnico per un backend documentale Azure AI. Rispondi in italiano, in modo chiaro e professionale."),
      new UserChatMessage(request.Input)
    };

    var result = await _chatClient.CompleteChatAsync(messages);
    var output = string.Concat(result.Value.Content.Select(c => c.Text));
    return new AgentResponse { Output = output };
    
  }

  async Task<AgentResponse> IAgentService.SummarizeAsync(AgentRequest request)
  {
    var messages = new List<ChatMessage>
    {
      new SystemChatMessage("Riassumi il testo in italiano in 5 punti chiave, con tono professionale."),
      new UserChatMessage(request.Input)
    };

    var result = await _chatClient.CompleteChatAsync(messages);
    var output = string.Concat(result.Value.Content.Select(c => c.Text));

    return new AgentResponse { Output = output };
  }
}
