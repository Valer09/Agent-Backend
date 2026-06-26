using System.ComponentModel.DataAnnotations;

namespace AI_Agent_Backend.Model;

public class AgentRequest
{
  [Required(ErrorMessage = "Input is required")]
  [MinLength(1, ErrorMessage = "Input cannot be empty")]
  [MaxLength(4000, ErrorMessage = "Input exceeds maximum allowed length of 4000 characters")]
  public string Input { get; set; } = string.Empty;
}