using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;
using resume_analyzer_api.Core;
using UglyToad.PdfPig;
using ChatRole = Microsoft.Extensions.AI.ChatRole;

namespace resume_analyzer_api.AI_Resume_Analyzing_Service;

public class ResumeAnalyzerOllama:IResumeAnalyzer
{
    private readonly ILogger<ResumeAnalyzerOllama> _logger;
    
    private readonly IChatClient _chatClient;

    public ResumeAnalyzerOllama(ILogger<ResumeAnalyzerOllama> logger, IChatClient chatClient)
    {
        _logger = logger;
        _chatClient = chatClient;
    }

    public async Task<string?> Analyze(Request request) {
        if (request.Resume is null || request.Resume.Length == 0) return null;
        PdfAnalysis analysis = await AnalyzeTextFromPdf(request.Resume);
        
        string structuredPrompt = $@"
RESUME ANALYSIS REPORT
=====================

1. CONTENT ANALYSIS:
-------------------
{analysis.Content}

2. FONT AND FORMATTING ANALYSIS:
------------------------------
{analysis.FontsAnalysis()}

3. PAGE ANALYSIS:
------------------------------
{analysis.PagesAnalysis()}
Considering this report for your analysis.
";

        try
        {
            ChatMessage prompt = new ChatMessage(ChatRole.User, new List<AIContent>
            {
                new TextContent($"PDF Content:\n{structuredPrompt}\n\nInstructions:\n{Prompt.Value}")
            });
            var response = await _chatClient.GetResponseAsync(
                new[] { prompt },
                new ChatOptions
                {
                    Temperature = 0.6f,
                    MaxOutputTokens = 1024
                }
            );
            var result = response.Text;
            return result;

        }
        catch (Exception e)
        {
            _logger.LogError($"An error occured when the sending the prompt to ollama; {e.Message}");
            throw;
        }
    }

    public Task<Response?> AnalyzeForPosition(Request request) {
        throw new NotImplementedException();
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
}