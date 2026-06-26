using AI_Agent_Backend.Model;

namespace AI_Agent_Backend.Service;

public interface IAgentService
{
  Task<AgentResponse> AskAsync(AgentRequest request);
  Task<AgentResponse> SummarizeAsync(AgentRequest request);
}