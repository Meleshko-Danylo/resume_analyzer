using resume_analyzer_api.Core;

namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public interface IResumeAnalyzer
{
    public Task<Response> Analyze(Request request);
    public Task<Response> AnalyzeForPosition(Request request);
}