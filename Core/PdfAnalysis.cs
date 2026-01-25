using System.Text;

namespace resume_analyzer_api.Core;

public class PdfAnalysis
{
    public string Content { get; set; } = String.Empty;
    public Dictionary<string, FontInfo> Fonts { get; set; } = new();
    public List<PageInfo> PagesInfo { get; set; } = new();
    public int PagesCount => PagesInfo.Count;

    public string PagesAnalysis()
    {
        var analysis = new StringBuilder();
        analysis.AppendLine($"Total Pages: {PagesCount}");
        analysis.AppendLine("Analysis for each page:");
        analysis.AppendLine();
        
        foreach (var page in PagesInfo)
        {
            analysis.AppendLine();
            analysis.AppendLine($"Page {page.PageNumber}");
            analysis.AppendLine($"Number of Images on the page: {page.ImageCount}");
            analysis.AppendLine($"Page width: {page.Width}");
            analysis.AppendLine($"Page height: {page.Height}");
            analysis.AppendLine();
        }
        return analysis.ToString();
    }

    public string FontsAnalysis()
    {
        var analysis = new StringBuilder();
        analysis.AppendLine($"Total Fonts: {Fonts.Count}\n");
        analysis.AppendLine("Analysis for each font:");
        
        var analysisPerEachFont = string.Join("\n",
            Fonts.Select(f => $"• {f.Value.FontName}: {f.Value.SizeRange}pt (used for {f.Value.Count} words)"));
        analysis.AppendLine(analysisPerEachFont);
        
        return analysis.ToString();
    }
}

public class FontInfo
{
    public string FontName { get; set; } = string.Empty;
    public double MinSize { get; set; } 
    public double MaxSize { get; set; }
    public int Count { get; set; }
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    
    public string SizeRange => MinSize == MaxSize ? 
        $"{MinSize:F1}" : 
        $"{MinSize:F1}-{MaxSize:F1}";
}

public class PageInfo
{
    public int PageNumber { get; set; }
    public int ImageCount { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}