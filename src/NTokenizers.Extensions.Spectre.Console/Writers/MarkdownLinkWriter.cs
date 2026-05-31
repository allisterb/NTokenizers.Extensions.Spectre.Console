using NTokenizers.Markdown.Metadata;
using Spectre.Console;

namespace NTokenizers.Extensions.Spectre.Console.Writers;

internal class MarkdownLinkWriter
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly string _linkMarkup;

    internal MarkdownLinkWriter(IAnsiConsole ansiConsole, Style linkStyle)
    {
        _ansiConsole = ansiConsole;
        _linkMarkup = linkStyle.ToMarkup();
    }

    internal void Write(LinkMetadata linkMeta)
    {
        if (linkMeta.Text is null)
        {
            // Link with URL as display text
            _ansiConsole.Markup($"[{_linkMarkup} link]{linkMeta.Url}[/]");
        }
        else
        {
            // Link with custom display text
            _ansiConsole.Markup($"[{_linkMarkup} link={linkMeta.Url}]{linkMeta.Text}[/]");
        }
    }
}