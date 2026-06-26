using AI_Agent_Backend.Model;
using AI_Agent_Backend.Service;
using Microsoft.AspNetCore.Mvc;

namespace AI_Agent_Backend.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class AgentController(IAgentService service) : ControllerBase
  {

    [HttpGet("health")]
    public String Health() => "OK";
    
    [HttpPost("ask")]
    public async Task<IResult> Ask([FromBody] AgentRequest rq) => 
      string.IsNullOrWhiteSpace(rq.Input) 
        ? InputRequired() 
        : Results.Ok(await service.AskAsync(rq));

    [HttpPost("summarize")]
    public async Task<IResult> Summarize([FromBody] AgentRequest rq) =>
      string.IsNullOrWhiteSpace(rq.Input) 
        ? InputRequired() 
        : Results.Ok(await service.SummarizeAsync(rq));

    private static IResult InputRequired() => Results.BadRequest(new { error = "Input is required" });
  }
  
}
