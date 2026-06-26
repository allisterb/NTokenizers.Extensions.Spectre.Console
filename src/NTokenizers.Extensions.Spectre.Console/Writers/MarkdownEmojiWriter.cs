using NTokenizers.Markdown.Metadata;
using Spectre.Console;

namespace NTokenizers.Extensions.Spectre.Console.Writers;

internal class MarkdownEmojiWriter(IAnsiConsole ansiConsole)
{
    internal void Write(EmojiMetadata emojiMeta)
    {
        ansiConsole.Markup($"[yellow]:{emojiMeta.Name}:[/]");
    }
}