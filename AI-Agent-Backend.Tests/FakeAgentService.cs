using AI_Agent_Backend.Model;
using AI_Agent_Backend.Service;

namespace AI_Agent_Backend.Tests;

/// <summary>
/// Fake implementation of IAgentService for smoke tests.
/// Returns predictable responses without requiring Azure credentials.
/// </summary>
public class FakeAgentService : IAgentService
{
  public Task<AgentResponse> AskAsync(AgentRequest request) =>
    Task.FromResult(new AgentResponse
    {
      Output = "Fake ask response",
      Operation = "ask",
      Model = "fake-model",
      RequestId = "test1234",
      TimestampUtc = DateTime.UtcNow,
      ElapsedMs = 0
    });

  public Task<AgentResponse> SummarizeAsync(AgentRequest request) =>
    Task.FromResult(new AgentResponse
    {
      Output = "Fake summarize response",
      Operation = "summarize",
      Model = "fake-model",
      RequestId = "test5678",
      TimestampUtc = DateTime.UtcNow,
      ElapsedMs = 0
    });
}
