
using AI_Agent_Backend.Configuration;
using AI_Agent_Backend.Model;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

namespace AI_Agent_Backend.Service;

internal class AgentService : IAgentService
{
  private readonly ChatClient _chatClient;
  
  private readonly ILogger<AgentService> _logger;
  private readonly AzureOpenAiOptions _openAiOptions;

  public AgentService(IOptions<AzureOpenAiOptions> options, ILogger<AgentService> logger)
  {

    var openAiOptions = options.Value;
    var clientOptions = new AzureOpenAIClientOptions(AzureOpenAIClientOptions.ServiceVersion.V2025_01_01_Preview);
    var client = new AzureOpenAIClient(new Uri(openAiOptions.Endpoint), new AzureKeyCredential(openAiOptions.ApiKey), clientOptions);

    _openAiOptions = openAiOptions;
    _logger = logger;
    _chatClient = client.GetChatClient(openAiOptions.DeploymentName);
  }

  public async Task<AgentResponse> AskAsync(AgentRequest request)
    => await ExecuteAsync("ask", request.Input, [
      new SystemChatMessage(
        "Sei un assistente tecnico per un backend documentale Azure AI. Rispondi in italiano, in modo chiaro e professionale."),
      new UserChatMessage(request.Input)
    ]);

  public async Task<AgentResponse> SummarizeAsync(AgentRequest request)
    => await ExecuteAsync("summarize", request.Input, [
      new SystemChatMessage("Riassumi il testo in italiano in 5 punti chiave, con tono professionale."),
      new UserChatMessage(request.Input)
    ]);


  private async Task<AgentResponse> ExecuteAsync(string operation, string inputPreview, List<ChatMessage> messages)
  {
    var requestId = Guid.NewGuid().ToString("N")[..8];
    var sw = Stopwatch.StartNew();

    _logger.LogInformation("[{RequestId}] [{Operation}] Starting — input length: {InputLength} chars", requestId, operation, inputPreview.Length);

    try
    {
      var result = await _chatClient.CompleteChatAsync(messages);
      var output = string.Concat(result.Value.Content.Select(c => c.Text));
      sw.Stop();

      _logger.LogInformation("[{RequestId}] [{Operation}] Completed in {ElapsedMs}ms — output length: {OutputLength} chars", requestId, operation, sw.ElapsedMilliseconds, output.Length);

      return new AgentResponse
      {
        Output = output,
        Operation = operation,
        Model = _openAiOptions.DeploymentName,
        RequestId = requestId,
        TimestampUtc = DateTime.UtcNow,
        ElapsedMs = sw.ElapsedMilliseconds
      };
    }
    catch (RequestFailedException ex)
    {
      sw.Stop();
      _logger.LogError(ex, "[{RequestId}] [{Operation}] Azure OpenAI request failed after {ElapsedMs}ms — Status: {StatusCode}", requestId, operation, sw.ElapsedMilliseconds, ex.Status);
      throw;
    }
    catch (Exception ex)
    {
      sw.Stop();
      _logger.LogError(ex, "[{RequestId}] [{Operation}] Unexpected error after {ElapsedMs}ms", requestId, operation, sw.ElapsedMilliseconds);
      throw;
    }
  }


}