using AI_Agent_Backend.Model;
using AI_Agent_Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace AI_Agent_Backend.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AgentController(IAgentService service, ILogger<AgentController> logger) : ControllerBase
  {

    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "OK", timestampUtc = DateTime.UtcNow });

    [HttpPost("ask")]
    public async Task<IResult> Ask([FromBody] AgentRequest rq)
    {
      logger.LogInformation("[Ask] Request received — input length: {InputLength}", rq.Input.Length);
      try
      {
        return Results.Ok(await service.AskAsync(rq));
      }
      catch (Exception ex)
      {
        return HandleException(ex, "ask");
      }
    }


    [HttpPost("summarize")]
    public async Task<IResult> Summarize([FromBody] AgentRequest rq)
    {
      logger.LogInformation("[Summarize] Request received — input length: {InputLength}", rq.Input.Length);
      try
      {
        return Results.Ok(await service.SummarizeAsync(rq));
      }
      catch (Exception ex)
      {
        return HandleException(ex, "summarize");
      }
    }

    
    
    private IResult HandleException(Exception ex, string operation) => ex switch
    {
      Azure.RequestFailedException { Status: 429 } e => LogAndReturn(
        () => logger.LogWarning("[{Op}] Rate limit hit: {Msg}", operation, e.Message),
        Results.Json(new { error = "Too many requests. Please retry later." }, statusCode: 429)),

      Azure.RequestFailedException e => LogAndReturn(
        () => logger.LogError(e, "[{Op}] Azure error: {Status}", operation, e.Status),
        Results.Json(new { error = "Azure OpenAI service error.", detail = e.Message }, statusCode: 502)),

      _ => LogAndReturn(
        () => logger.LogError(ex, "[{Op}] Unexpected error", operation),
        Results.Json(new { error = "Internal server error." }, statusCode: 500))
    };

    private static IResult LogAndReturn(Action logAction, IResult result)
    {
      logAction();
      return result;
    }
  }
  
}
