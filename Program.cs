using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Mvc;
using OllamaSharp;
using resume_analyzer_api.AI_Resume_Analyzing_Service;
using resume_analyzer_api.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000", "https://localhost:3000", "https://zealous-desert-0aa692a03.6.azurestaticapps.net");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });
});
builder.Services.AddHttpClient("Ollama", client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromMinutes(5);
});
// builder.Services.AddChatClient(new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.1:latest"));
builder.Services.AddSingleton(new Client(apiKey:builder.Configuration["GOOGLE_API_KEY"], httpOptions: new HttpOptions
{
    Timeout = 5 * 60 * 1000
}));
builder.Services.AddProblemDetails(opt => opt.CustomizeProblemDetails = context =>
{
    context.ProblemDetails.Title = "Resume Analyzer API";
    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
});
builder.Services.AddScoped<IResumeAnalyzer<Response>, ResumeAnalyzerGemini<Response>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapPost("/resume-analyzer-api/analyze-resume", async ([FromForm] Request body, IResumeAnalyzer<Response> analyzer, ILogger<Program> logger, CancellationToken ct) =>
{
    try
    {
        var result = await analyzer.Analyze(body, ct);
        logger.LogInformation("Resume analyzer analysis complete");
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        logger.LogError(e, "An error occured in resume analyzer Analyze method");
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}).DisableAntiforgery();

app.MapPost("/resume-analyzer-api/analyze-resume-for-position", async ([FromForm] Request body, IResumeAnalyzer<Response> analyzer, ILogger<Program> logger, CancellationToken ct) =>
{
    try
    {
        var result = await analyzer.AnalyzeDetailed(body, ct);
        logger.LogInformation("Resume analyzer analysis complete");
        return Results.Ok(result);
    }
    catch (Exception e)
    {
        logger.LogError(e, "An error occured in resume analyzer Analyze method");
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}).DisableAntiforgery();

app.UseCors("DefaultPolicy");

app.Run();
