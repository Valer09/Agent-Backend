using AI_Agent_Backend.Model;
using AI_Agent_Backend.Service;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Load secrets from Azure Key Vault in production
var keyVaultUri = builder.Configuration["KeyVaultUri"];
if (!string.IsNullOrWhiteSpace(keyVaultUri) && builder.Environment.IsProduction())
{
  builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUri),
    new DefaultAzureCredential());
}

// Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAgentService, AgentService>();

var app = builder.Build();

// Swagger / OpenAPI
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "API v1");
  });

  app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();

//TODO: These endpoints declarations are going to be replaced by controllers
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost("/agent/ask", async ([FromBody] AgentRequest rq, [FromServices] IAgentService service) => await HandleBasicRequest(rq, service));
app.MapPost("/agent/summarize", async ([FromBody] AgentRequest rq, [FromServices] IAgentService service) => await HandleBasicRequest(rq, service));

app.MapControllers();

app.Run();

static async Task<IResult> HandleBasicRequest(AgentRequest agentRequest, IAgentService agentService)
{
  if (string.IsNullOrWhiteSpace(agentRequest.Input))
  {
    return Results.BadRequest(new { error = "Input is required" });
  }

  var res = await agentService.AskAsync(agentRequest);
  return Results.Ok(res);
}