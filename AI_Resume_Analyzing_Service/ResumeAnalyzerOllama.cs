using System.Text;
using System.Text.Json;
using Microsoft.Extensions.AI;
using OllamaSharp;
using resume_analyzer_api.Core;
using UglyToad.PdfPig;
using ChatRole = Microsoft.Extensions.AI.ChatRole;

namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public class ResumeAnalyzerOllama<T>:IResumeAnalyzer<T> where T: class
{
    private readonly ILogger<ResumeAnalyzerOllama<T>> _logger;
    
    private readonly IChatClient _chatClient;
    private readonly IHttpClientFactory _httpClientFactory;

    public ResumeAnalyzerOllama(ILogger<ResumeAnalyzerOllama<T>> logger, IChatClient chatClient, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _chatClient = chatClient;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T?> Analyze(Request request, CancellationToken cancellationToken = default) {
        if (request.Resume is null || request.Resume.Length == 0) return null;
        PdfAnalysis analysis = await AnalyzeTextFromPdf(request.Resume, cancellationToken);
        
        string structuredPrompt = $@"
Analyze the resume and return ONLY valid JSON.
DO NOT include markdown.
DO NOT include explanations.
DO NOT include code fences.
DO NOT include text outside JSON.

RESUME ANALYSIS REPORT
=====================

1. CONTENT ANALYSIS:
-------------------
{analysis.Content}

2. FONT AND FORMATTING ANALYSIS:
------------------------------
{analysis.FontsAnalysis()}

Considering this report for your analysis.
";

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Ollama");

            var chatClient = new OllamaApiClient(
                client: httpClient,
                defaultModel:"llama3.1:latest"
            );
            
            ChatMessage prompt = new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent($"PDF Content:\n{structuredPrompt}\n\nInstructions:\n{Prompt.Value}")
            });

            var response = await chatClient.GetResponseAsync<Response>(new[] { prompt },
                new ChatOptions
                {
                    Temperature = 0.2f,
                    MaxOutputTokens = 1500,
                    ResponseFormat = ChatResponseFormat.Json
                }, cancellationToken: cancellationToken);
            
            var jsonString = ExtractJsonObject(response.Text);
            var result = Deserialize(jsonString);
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured when the sending the prompt to ollama; {e.Message}");
            throw;
        }
    }

    public async Task<T?> AnalyzeDetailed(Request request, CancellationToken cancellationToken = default) {
        if (request.Resume is null || request.Resume.Length == 0) return null;
        PdfAnalysis analysis = await AnalyzeTextFromPdf(request.Resume, cancellationToken);
        
        string structuredPrompt = $@"
Analyze the resume and return ONLY valid JSON.
DO NOT include markdown.
DO NOT include explanations.
DO NOT include code fences.
DO NOT include text outside JSON.

{request.PositionDescription}

RESUME ANALYSIS REPORT
=====================

1. CONTENT ANALYSIS:
-------------------
{analysis.Content}

2. FONT AND FORMATTING ANALYSIS:
------------------------------
{analysis.FontsAnalysis()}

Considering this report for your analysis.
";

        try
        {
            var httpClient = _httpClientFactory.CreateClient("Ollama");

            var chatClient = new OllamaApiClient(
                client: httpClient,
                defaultModel:"llama3.1:latest"
            );
            
            ChatMessage prompt = new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent($"PDF Content:\n{structuredPrompt}\n\nInstructions:\n{Prompt.Value}")
            });

            var response = await chatClient.GetResponseAsync<Response>(new[] { prompt },
                new ChatOptions
                {
                    Temperature = 0.2f,
                    MaxOutputTokens = 1500,
                    ResponseFormat = ChatResponseFormat.Json
                }, cancellationToken: cancellationToken);
            
            var jsonString = ExtractJsonObject(response.Text);
            var result = Deserialize(jsonString);
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured when the sending the prompt to ollama; {e.Message}");
            throw;
        }
    }

    private async Task<PdfAnalysis> AnalyzeTextFromPdf(IFormFile pdf, CancellationToken cancellationToken)
    {
        var content = new StringBuilder();
        var pdfAnalysis = new PdfAnalysis();
        var fonts = pdfAnalysis.Fonts;

        using (var memory = new MemoryStream())
        {
            await pdf.CopyToAsync(memory, cancellationToken);
            memory.Position = 0;

            using (var pdfDoc = PdfDocument.Open(memory))
            {
                foreach (var page in pdfDoc.GetPages()) {
                    cancellationToken.ThrowIfCancellationRequested();
                    content.AppendLine(page.Text);
                    var pageInfo = new PageInfo()
                    {
                        PageNumber = page.Number,
                        ImageCount = page.NumberOfImages,
                        Width = page.Width,
                        Height = page.Height,
                    };
                    pdfAnalysis.PagesInfo.Add(pageInfo);
                    
                    foreach (var word in page.GetWords())
                    {
                        if(word.FontName is null) continue;
                        var isFontUsed = fonts.ContainsKey(word.FontName);
                        if (isFontUsed) fonts[word.FontName].Count++;
                        
                        foreach (var letter in word.Letters)
                        {
                            if(letter.FontName is null) continue;
                            
                            if (!pdfAnalysis.Fonts.ContainsKey(letter.FontName))
                            {
                                var font = new FontInfo
                                {
                                    FontName = letter.FontName,
                                    MinSize = letter.FontSize,
                                    MaxSize = letter.FontSize,
                                    Count = 1,
                                    IsBold = letter.FontName.Contains("Bold") || letter.FontName.Contains("bold"),
                                    IsItalic = letter.FontName.Contains("Italic") || letter.FontName.Contains("italic")
                                };
                                pdfAnalysis.Fonts.Add(letter.FontName, font);
                            }
                            else
                            {
                                var font = fonts[letter.FontName];
                                font.MinSize = Math.Min(font.MinSize, letter.FontSize);
                                font.MaxSize = Math.Max(font.MaxSize, letter.FontSize);
                            }
                        }
                    }
                }
                
            }
        }
        pdfAnalysis.Content = content.ToString();
        
        return pdfAnalysis;
    }

    private string ExtractJsonObject(string text)
    {
        var start = text.IndexOf('{');
        var end = text.LastIndexOf('}');

        if (start >= end || start < 0 || end < 0)
            throw new InvalidOperationException("No Json found in the text");
        
        return text.Substring(start, end - start + 1);
    }

    private T Deserialize(string jsonStrong)
    {
        var result = JsonSerializer.Deserialize<T>(jsonStrong, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (result is null) throw new NullReferenceException("Failed to parse AI JSON output.");
        
        return result;
    }
}