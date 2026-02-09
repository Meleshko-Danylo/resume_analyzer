using System.Text;
using System.Text.Json;
using Google.GenAI;
using Google.GenAI.Types;
using resume_analyzer_api.Core;
using UglyToad.PdfPig;

namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public class ResumeAnalyzerGemini<T>:IResumeAnalyzer<T> where T: class
{
    private readonly ILogger<ResumeAnalyzerOllama<T>> _logger;
    
    private readonly Client _chatClient;

    public ResumeAnalyzerGemini(ILogger<ResumeAnalyzerOllama<T>> logger, Client chatClient)
    {
        _logger = logger;
        _chatClient = chatClient;
    }

    public async Task<T?> Analyze(Request request, CancellationToken cancellationToken = default) {
        if (request.Resume is null || request.Resume.Length == 0) return null;
        PdfAnalysis analysis = await AnalyzeTextFromPdf(request.Resume);
        
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
            var model = "gemini-3-flash-preview";
            var contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = new List<Part>
                    {
                        new Part { Text = $"PDF Content:\n{structuredPrompt}\n\nInstructions:\n{Prompt.Value}" },
                    }
                },
            };
            
            var config = new GenerateContentConfig
            {
                ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = ThinkingLevel.HIGH
                },
                ResponseMimeType = "application/json",
                Temperature = 0.6f,
                MaxOutputTokens = 2048,
            };

            var response = await _chatClient.Models.GenerateContentAsync(model, contents, config);
            
            var jsonString = ExtractJsonObject(response.Candidates?[0].Content?.Parts?[0].Text);
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
        PdfAnalysis analysis = await AnalyzeTextFromPdf(request.Resume);
        
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
            var model = "gemini-3-flash-preview";
            var contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = new List<Part>
                    {
                        new Part { Text = $"PDF Content:\n{structuredPrompt}\n\nInstructions:\n{Prompt.Value}" },
                    }
                },
            };
            
            var config = new GenerateContentConfig
            {
                ThinkingConfig = new ThinkingConfig
                {
                    ThinkingLevel = ThinkingLevel.HIGH
                },
                ResponseMimeType = "application/json",
                Temperature = 0.6f,
                MaxOutputTokens = 2048,
            };

            var response = await _chatClient.Models.GenerateContentAsync(model, contents, config);
            
            var jsonString = ExtractJsonObject(response.Candidates?[0].Content?.Parts?[0].Text);
            var result = Deserialize(jsonString);
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured when the sending the prompt to ollama; {e.Message}");
            throw;
        }
    }

    private async Task<PdfAnalysis> AnalyzeTextFromPdf(IFormFile pdf)
    {
        var content = new StringBuilder();
        var pdfAnalysis = new PdfAnalysis();
        var fonts = pdfAnalysis.Fonts;

        using (var memory = new MemoryStream())
        {
            await pdf.CopyToAsync(memory);
            memory.Position = 0;

            using (var pdfDoc = PdfDocument.Open(memory))
            {
                foreach (var page in pdfDoc.GetPages()) {
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

    private string ExtractJsonObject(string? text)
    {
        if(text is null) return string.Empty;
        
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