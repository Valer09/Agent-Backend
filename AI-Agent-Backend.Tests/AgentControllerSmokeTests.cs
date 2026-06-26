using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using AI_Agent_Backend.Service;
using AI_Agent_Backend.Model;
using Microsoft.Extensions.Configuration;

namespace AI_Agent_Backend.Tests;

public class AgentControllerSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public AgentControllerSmokeTests(WebApplicationFactory<Program> factory)
  {
    _client = factory.WithWebHostBuilder(builder =>
      {
        builder.ConfigureAppConfiguration((context, config) =>
        {
          config.AddInMemoryCollection(new Dictionary<string, string?>
          {
            ["AzureOpenAI:Endpoint"] = "https://placeholder.openai.azure.com/",
            ["AzureOpenAI:ApiKey"] = "placeholder-key",
            ["AzureOpenAI:DeploymentName"] = "placeholder-deployment"
          });
        });

        builder.ConfigureServices(services => services.AddScoped<IAgentService, FakeAgentService>());
      }
    
      ).CreateClient();
  }

  [Fact]
  public async Task Health_Returns200AndOkStatus()
  {
    var response = await _client.GetAsync("/agent/health");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var body = await response.Content.ReadFromJsonAsync<HealthResponse>();
    Assert.Equal("OK", body?.Status);
  }

  [Fact]
  public async Task Ask_WithValidInput_Returns200()
  {
    var response = await _client.PostAsJsonAsync("/agent/ask", new AgentRequest { Input = "test input" });

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var body = await response.Content.ReadFromJsonAsync<AgentResponse>();
    Assert.NotNull(body?.Output);
    Assert.Equal("ask", body?.Operation);
    Assert.NotEmpty(body?.RequestId ?? "");
  }

  [Fact]
  public async Task Summarize_WithValidInput_Returns200()
  {
    var response = await _client.PostAsJsonAsync("/agent/summarize", new AgentRequest { Input = "test input" });

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var body = await response.Content.ReadFromJsonAsync<AgentResponse>();
    Assert.Equal("summarize", body?.Operation);
  }

  [Fact]
  public async Task Ask_WithEmptyInput_Returns400()
  {
    var response = await _client.PostAsJsonAsync("/agent/ask", new AgentRequest { Input = "" });

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task Ask_WithNullBody_Returns400()
  {
    var response = await _client.PostAsync("/agent/ask",
      new StringContent("{}", System.Text.Encoding.UTF8, "application/json"));

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task Ask_WithInputExceedingMaxLength_Returns400()
  {
    var response = await _client.PostAsJsonAsync("/agent/ask",
      new AgentRequest { Input = new string('x', 4001) });

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  // Helper for health response deserialization
  private record HealthResponse(string Status, DateTime TimestampUtc);
}
