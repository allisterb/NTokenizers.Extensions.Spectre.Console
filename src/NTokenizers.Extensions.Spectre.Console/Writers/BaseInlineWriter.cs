using NTokenizers.Core;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Text;

namespace NTokenizers.Extensions.Spectre.Console.Writers;

internal abstract class BaseInlineWriter<TToken, TTokentype> where TToken : IToken<TTokentype> where TTokentype : Enum
{
    internal protected readonly IAnsiConsole _ansiConsole;
    internal protected readonly LiveDisplayContext? _liveDisplayContext;
    internal protected readonly Paragraph _liveParagraph;

    internal BaseInlineWriter(IAnsiConsole ansiConsole)
    {
        _ansiConsole = ansiConsole;
        _liveDisplayContext = null;
        _liveParagraph = new("");
    }

    internal BaseInlineWriter(IAnsiConsole ansiConsole, Paragraph? liveParagraph, LiveDisplayContext? ctx)
    {
        _ansiConsole = ansiConsole;
        _liveParagraph = liveParagraph ?? new("");
        _liveDisplayContext = ctx;
    }

    protected virtual Style GetStyle(TTokentype token) => Style.Plain;

    internal void WriteToken(TToken token)
    {
        _ansiConsole.Write(new Markup(Markup.Escape(token.Value), GetStyle(token.TokenType)));
    }

    internal void WriteTokenInLiveTarget(TToken token)
    {
        WriteToken(_liveParagraph, token);
        _liveDisplayContext?.Refresh();
    }

    internal async Task WriteAsync(InlineMetadata<TToken> metadata)
    {
        // Accumulate the streamed tokens into the paragraph, then write the built renderable once. The upstream
        // implementation drove this with a LiveDisplay and a per-token ctx.Refresh(), which corrupts output when the
        // target is a static offscreen ConsoleBuffer (Jumbee's AnsiConsoleBuffer): a LiveDisplay redraws each frame
        // assuming an interactive terminal it can cursor-up/erase, so every refresh overlays a differently-sized,
        // never-erased frame and the (top) border cells accumulate into garbage. A one-shot write renders cleanly.
        await StartedAsync(metadata);
        await metadata.RegisterInlineTokenHandler(async inlineToken =>
        {
            await WriteTokenAsync(_liveParagraph, inlineToken, null);
        });
        await FinalizeAsync(metadata);
        _ansiConsole.Write(GetIRendable());
    }

    protected virtual IRenderable GetIRendable() => 
        new Panel(_liveParagraph)
            .Border(new LeftBoxBorder())
            .BorderStyle(new Style(Color.Green));

    protected virtual Task StartedAsync(InlineMetadata<TToken> metadata) => Task.CompletedTask;

    protected virtual Task FinalizeAsync(InlineMetadata<TToken> metadata) => Task.CompletedTask;

    protected virtual Task WriteTokenAsync(Paragraph? liveParagraph, TToken token, LiveDisplayContext? ctx)
    {
        WriteToken(liveParagraph, token);
        return Task.CompletedTask;
    }

    protected virtual void WriteToken(Paragraph? liveParagraph, TToken token)
    {
        if (token.Value is not null)
        {
            Debug.WriteLine($"Writing token: `{token.Value}` of type `{token.TokenType}`");

            if (liveParagraph is null)
            {
                _ansiConsole.Write(new Markup(Markup.Escape(token.Value), GetStyle(token.TokenType)));
            }
            else
            {
                liveParagraph.Append(token.Value, GetStyle(token.TokenType));
            }
        }
    }

    public void Parse(BaseSubTokenizer<TToken> tokenizer, string value)
    {
        Task.Run(async () =>
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            await tokenizer.ParseAsync(stream, this.WriteToken);
        }).GetAwaiter().GetResult();
    }

    public void Parse(BaseTokenizer<TToken> tokenizer, string value)
    {
        Task.Run(async () =>
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            await tokenizer.ParseAsync(stream, this.WriteToken);
        }).GetAwaiter().GetResult();
    }
}
