using resume_analyzer_api.Core;

namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public interface IResumeAnalyzer<T> where T : class
{
    public Task<T?> Analyze(Request request, CancellationToken ct);
    public Task<T?> AnalyzeDetailed(Request request, CancellationToken ct);
}