using NTokenizers.Extensions.Spectre.Console.Styles;
using NTokenizers.Extensions.Spectre.Console.Writers;
using NTokenizers.Markdown;
using Spectre.Console;
using System.IO;
using System.Text;

namespace NTokenizers.Extensions.Spectre.Console;

/// <summary>
/// Provides extension methods for <see cref="IAnsiConsole"/> to render markdown text with syntax highlighting.
/// </summary>
public static class AnsiConsoleMarkdownExtensions
{
    /// <summary>
    /// Writes markdown text to the console asynchronously using specified markdown styles and returns the parsed string.
    /// </summary>
    /// <param name="ansiConsole">The ANSI console to write to.</param>
    /// <param name="stream">The stream containing the markdown text.</param>
    /// <param name="markdownStyles">The markdown styles to use for rendering.</param>
    /// <param name="encoding">The character encoding to use. If null, encoding will be detected from the stream's byte order mark (BOM).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous write operation and contains the parsed string.</returns>
    public static async Task<string> WriteMarkdownAsync(this IAnsiConsole ansiConsole, Stream stream, MarkdownStyles? markdownStyles = null, Encoding? encoding = null, CancellationToken ct = default, bool inline = false)
    {
        var markdownWriter = MarkdownWriter.Create(ansiConsole);
        var markdownInlineWriter = MarkdownInlineWriter.Create(ansiConsole);
        markdownWriter.MarkdownStyles = markdownStyles ?? MarkdownStyles.Default;
        if (encoding is null)
        {
            // Call overload without encoding to preserve BOM detection
            if (!inline)
            {
                return await MarkdownTokenizer.Create().ParseAsync(
                    stream,
                    ct,
                    async token => await markdownWriter.WriteAsync(token)
                );
            }
            else
            {
                return await MarkdownTokenizer.Create().ParseAsync(
                    stream,
                    ct,
                    token => markdownInlineWriter.WriteToken(token)
                );
            }
        }
        else
        {

            // Call overload with encoding and cancellation token
            if (!inline)
            {
                return await MarkdownTokenizer.Create().ParseAsync(
                    stream,
                    encoding,
                    ct,
                    async token => await markdownWriter.WriteAsync(token)
                );
            }
            else
            {
                return await MarkdownTokenizer.Create().ParseAsync(
                    stream,
                    encoding,
                    ct,
                    token => markdownInlineWriter.WriteToken(token)
                );
            }
        }
    }

    /// <summary>
    /// Writes markdown text to the console synchronously using specified markdown styles and returns the parsed string.
    /// </summary>
    /// <param name="ansiConsole">The ANSI console to write to.</param>
    /// <param name="stream">The stream containing the markdown text.</param>
    /// <param name="markdownStyles">The markdown styles to use for rendering.</param>
    /// <param name="encoding">The character encoding to use. If null, encoding will be detected from the stream's byte order mark (BOM).</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The parsed string from the input stream.</returns>
    public static string WriteMarkdown(this IAnsiConsole ansiConsole, Stream stream, MarkdownStyles? markdownStyles = null, Encoding? encoding = null, CancellationToken ct = default)
    {
        var t = Task.Run(() => WriteMarkdownAsync(ansiConsole, stream, markdownStyles, encoding, ct), ct);
        return t.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Writes markdown text to the console synchronously using default markdown styles.
    /// </summary>
    /// <param name="ansiConsole">The ANSI console to write to.</param>
    /// <param name="value">The markdown text to write.</param>
    public static void WriteMarkdown(this IAnsiConsole ansiConsole, string value) =>
        WriteMarkdown(ansiConsole, value, MarkdownStyles.Default);

    /// <summary>
    /// Writes markdown text to the console synchronously using specified markdown styles.
    /// </summary>
    /// <param name="ansiConsole">The ANSI console to write to.</param>
    /// <param name="value">The markdown text to write.</param>
    /// <param name="markdownStyles">The markdown styles to use for rendering.</param>
    public static void WriteMarkdown(this IAnsiConsole ansiConsole, string value, MarkdownStyles markdownStyles)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        var t = Task.Run(() => WriteMarkdownAsync(ansiConsole, stream, markdownStyles, Encoding.UTF8, default));
        t.GetAwaiter().GetResult();
    }


    public static async Task WriteMarkdownInlineAsync(this IAnsiConsole ansiConsole, string value)
    {
        var writer = new MarkdownInlineWriter(ansiConsole);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        MarkdownTokenizer tokenizer = MarkdownTokenizer.Create();
        await tokenizer.ParseAsync(
                stream,                
                writer.WriteToken
            );
    }

    /// <summary>
    /// Parses and writes the specified Markdown string as inline content using the provided ANSI console and optional
    /// Markdown styles.
    /// </summary>
    /// <param name="ansiConsole">The ANSI console used for output.</param>
    /// <param name="value">The Markdown string to parse and write.</param>
    /// <param name="markdownStyles">Optional styles to apply to the Markdown content.</param>
    /// <returns>A string containing the rendered inline Markdown.</returns>
    public static void WriteMarkdownInline(this IAnsiConsole ansiConsole, string value)
    {
        var t = Task.Run(() => WriteMarkdownInlineAsync(ansiConsole, value));
        t.GetAwaiter().GetResult();
    }
}