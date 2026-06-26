namespace AI_Agent_Backend.Model;

public class AgentResponse
{
  public string Output { get; set; } = string.Empty;
  public string Operation { get; set; } = string.Empty;
  public string Model { get; set; } = string.Empty;
  public string RequestId { get; set; } = string.Empty;
  public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
  public long ElapsedMs { get; set; }
}