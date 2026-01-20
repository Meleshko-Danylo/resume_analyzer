namespace resume_analyzer_api.Core;

public class Response
{
    public double OverallScore { get; set; }
    public string ShortSummary { get; set; } = string.Empty;
    public List<Suggestion> Suggestions { get; set; } = new();
}

public class Suggestion
{
    public string Category  { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Good { get; set; } = string.Empty;
    public string Bad { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
}