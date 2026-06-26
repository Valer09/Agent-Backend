using System.ComponentModel.DataAnnotations;

namespace AI_Agent_Backend.Configuration;

internal class AzureOpenAiOptions
{
  public const string SectionName = "AzureOpenAI";


  [Required(ErrorMessage = "AzureOpenAI:Endpoint is required")]
  [Url(ErrorMessage = "AzureOpenAI:Endpoint must be a valid URL")]
  public string Endpoint { get; set; } = string.Empty;

  [Required(ErrorMessage = "AzureOpenAI:ApiKey is required")]
  public string ApiKey { get; set; } = string.Empty;

  [Required(ErrorMessage = "AzureOpenAI:DeploymentName is required")]
  public string DeploymentName { get; set; } = string.Empty;
}