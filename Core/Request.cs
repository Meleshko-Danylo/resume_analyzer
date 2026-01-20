namespace resume_analyzer_api.Core;

public class Request
{
    public IFormFile? Resume { get; set; } = null;
    public string? PositionDescription { get; set; } = String.Empty;
}