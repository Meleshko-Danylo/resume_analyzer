using resume_analyzer_api.Core;

namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public class ResumeAnalyzerOllama:IResumeAnalyzer
{
    public Task<Response> Analyze(Request request)
    {
        throw new NotImplementedException();
    }

    public Task<Response> AnalyzeForPosition(Request request)
    {
        throw new NotImplementedException();
    }
}