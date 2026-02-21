using Markdig;
using Markdig.Extensions.Globalization;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Piranha.Extend;
using Xunit;

namespace Piranha.Tests.Services;

public class MarkdownHighlightingTests
{
    private DefaultMarkdown CreateHighlightingMarkdown()
    {
        var markdown = new DefaultMarkdown();
        markdown.ConfigurePipeline(builder =>
        {
            // Enable advanced extensions which includes fenced code blocks
            builder.UseAdvancedExtensions();
            // Add our custom renderer
            builder.Use(new CodeHighlightingExtension());
        });
        return markdown;
    }

    [Fact]
    [Trait("Category", "Markdown")]
    [Trait("Feature", "SyntaxHighlighting")]
    public void LanguageClassesAreInjected()
    {
        var markdown = CreateHighlightingMarkdown();
        var mdInput = @"```csharp
public class HelloWorld { }
```";

        var html = markdown.Transform(mdInput);

        Assert.Contains("class=\"language-csharp\"", html);
    }

    [Fact]
    [Trait("Category", "Markdown")]
    [Trait("Feature", "SyntaxHighlighting")]
    public void JsonLanguageWorks()
    {
        var markdown = CreateHighlightingMarkdown();
        var mdInput = @"```json
{ ""key"": ""value"" }
```";

        var html = markdown.Transform(mdInput);

        Assert.Contains("class=\"language-json\"", html);
    }

    [Fact]
    [Trait("Category", "Markdown")]
    [Trait("Feature", "SyntaxHighlighting")]
    public void NoLanguageLabelWorks()
    {
        var markdown = CreateHighlightingMarkdown();
        var mdInput = @"```
plain text
```";

        var html = markdown.Transform(mdInput);

        Assert.Contains("<pre><code>", html);
    }
}

internal class CodeHighlightingExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer html)
        {
            var codeBlockRenderer = new CodeBlockRenderer();
            var existing = html.ObjectRenderers.FirstOrDefault(x => x.GetType().Name == "HtmlFencedCodeBlockRenderer");
            if (existing != null)
            {
                html.ObjectRenderers.Remove(existing);
            }
            html.ObjectRenderers.Add(codeBlockRenderer);
        }
    }
}

internal class CodeBlockRenderer : HtmlObjectRenderer<FencedCodeBlock>
{
    private static readonly Dictionary<String, String> LanguageMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "cs", "csharp" },
        { "c#", "csharp" },
        { "js", "javascript" },
        { "ts", "typescript" },
        { "py", "python" },
        { "sh", "bash" },
        { "ps", "powershell" },
        { "ps1", "powershell" },
    };

    private static String NormalizeLanguage(String language)
    {
        if (String.IsNullOrWhiteSpace(language))
            return String.Empty;
        language = language.Trim().Split(' ')[0].Trim();
        return LanguageMap.TryGetValue(language, out var normalized) ? normalized : language;
    }

    protected override void Write(HtmlRenderer renderer, FencedCodeBlock block)
    {
        renderer.EnsureLine();
        var language = NormalizeLanguage(block.Info);
        renderer.Write("<pre><code");
        if (!String.IsNullOrEmpty(language))
        {
            renderer.Write($" class=\"language-{System.Net.WebUtility.HtmlEncode(language)}\"");
        }
        renderer.WriteLine(">");
        var content = block.Lines.ToString();
        renderer.WriteEscapeUrl(content);
        renderer.WriteLine();
        renderer.WriteLine("</code></pre>");
    }
}
