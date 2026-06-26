using AI_Agent_Backend.Configuration;
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

// Options pattern with startup validation
builder.Services
  .AddOptions<AzureOpenAiOptions>()
  .Bind(builder.Configuration.GetSection(AzureOpenAiOptions.SectionName))
  .ValidateDataAnnotations()
  .ValidateOnStart();

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
app.MapControllers();
app.Run();
