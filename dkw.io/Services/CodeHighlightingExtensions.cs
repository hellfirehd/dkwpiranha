using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace RazorWeb.Services;

/// <summary>
/// Custom HTML renderer for fenced code blocks that preserves language info as a CSS class.
/// </summary>
public class CodeBlockRenderer : HtmlObjectRenderer<FencedCodeBlock>
{
    /// <summary>
    /// Writes the HTML for a fenced code block with language class.
    /// </summary>
    protected override void Write(HtmlRenderer renderer, FencedCodeBlock block)
    {
        renderer.EnsureLine();

        // Extract language from the fence info string (first token)
        var language = block.Info?.Trim().Split(' ')[0].Trim() ?? String.Empty;

        // Open the code block with optional language class
        renderer.Write("<pre><code");
        if (!String.IsNullOrEmpty(language))
        {
            renderer.Write($" class=\"language-{System.Net.WebUtility.HtmlEncode(language)}\"");
        }
        renderer.WriteLine(">");

        // Write the code content (HTML-escaped, not URL-encoded)
        var content = block.Lines.ToString();
        renderer.WriteEscape(content);

        // Close the code block
        renderer.WriteLine();
        renderer.WriteLine("</code></pre>");
    }
}

/// <summary>
/// Extension method to register the custom code block renderer with Markdig.
/// </summary>
public static class CodeHighlightingExtensions
{
    /// <summary>
    /// Configures Markdig to use language classes for syntax highlighting.
    /// </summary>
    public static MarkdownPipelineBuilder UseCodeHighlighting(this MarkdownPipelineBuilder builder)
    {
        builder.Extensions.Add(new CodeHighlightingExtension());
        return builder;
    }
}

/// <summary>
/// Markdig extension that registers the custom code block renderer.
/// </summary>
internal class CodeHighlightingExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // Nothing to do on pipeline setup
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is HtmlRenderer html)
        {
            // Find and replace the existing FencedCodeBlock renderer
            var existing = html.ObjectRenderers.FirstOrDefault(r => r is HtmlObjectRenderer<FencedCodeBlock>);

            if (existing != null)
            {
                var index = html.ObjectRenderers.IndexOf(existing);
                html.ObjectRenderers.RemoveAt(index);
                html.ObjectRenderers.Insert(index, new CodeBlockRenderer());
            }
            else
            {
                // If no existing renderer, add ours at the beginning
                html.ObjectRenderers.Insert(0, new CodeBlockRenderer());
            }
        }
    }
}
