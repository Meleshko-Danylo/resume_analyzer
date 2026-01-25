using Microsoft.AspNetCore.Mvc;
using OllamaSharp;
using resume_analyzer_api.AI_Resume_Analyzing_Service;
using resume_analyzer_api.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddChatClient(new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3:8b"));
builder.Services.AddProblemDetails(opt => opt.CustomizeProblemDetails = context =>
{
    context.ProblemDetails.Title = "Resume Analyzer API";
    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
});
builder.Services.AddScoped<IResumeAnalyzer, ResumeAnalyzerOllama>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapPost("/resume-analyzer-api/analyze-resume", async ([FromForm] Request body, IResumeAnalyzer analyzer, ILogger<Program> logger) =>
{
    try
    {
        // var analyzer = app.Services.GetRequiredService<IResumeAnalyzer>();
        var result = await analyzer.Analyze(body);
        logger.LogInformation("Resume analyzer analysis complete");
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        logger.LogError(e, "An error occured in resume analyzer Analyze method");
        throw;
    }
}).DisableAntiforgery();

app.MapPost("/resume-analyzer-api/analyze-resume-for-position", async (Request body, IResumeAnalyzer analyzer, ILogger<Program> logger) =>
{
    try
    {
        // var analyzer = app.Services.GetRequiredService<IResumeAnalyzer>();
        var result = await analyzer.AnalyzeForPosition(body);
        logger.LogInformation("Resume analyzer analysis complete");
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        logger.LogError(e, "An error occured in resume analyzer Analyze method");
        throw;
    }
}).DisableAntiforgery();

app.UseCors(policyBuilder =>
    policyBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
    // .WithOrigins("localhost:3000"));

app.Run();
