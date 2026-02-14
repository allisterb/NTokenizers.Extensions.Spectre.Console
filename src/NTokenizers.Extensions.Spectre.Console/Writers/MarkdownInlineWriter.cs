namespace NTokenizers.Extensions.Spectre.Console.Writers;

using global::Spectre.Console;
using NTokenizers.Extensions.Spectre.Console.Styles;
using NTokenizers.Markdown;
using NTokenizers.Sql;
using System;
using System.Collections.Generic;
using System.Text;

internal class MarkdownInlineWriter(IAnsiConsole ansiConsole, MarkdownStyles? styles = null) : BaseInlineWriter<MarkdownToken, MarkdownTokenType>(ansiConsole)
{
    protected override Style GetStyle(MarkdownTokenType token) => token switch
    {
        MarkdownTokenType.Heading => styles?.Heading ?? MarkdownStyles.Default.Heading,
        MarkdownTokenType.Bold => styles?.Bold ?? MarkdownStyles.Default.Bold,
        MarkdownTokenType.Italic => MarkdownStyles.Default.Italic,
        MarkdownTokenType.HorizontalRule => MarkdownStyles.Default.HorizontalRule,
        MarkdownTokenType.CodeInline => MarkdownStyles.Default.CodeInline,
        MarkdownTokenType.CodeBlock => MarkdownStyles.Default.CodeBlock,
        MarkdownTokenType.Link => MarkdownStyles.Default.Link,
        MarkdownTokenType.Image => MarkdownStyles.Default.Image,
        MarkdownTokenType.Blockquote => MarkdownStyles.Default.Blockquote,
        MarkdownTokenType.UnorderedListItem => MarkdownStyles.Default.UnorderedListItem,
        MarkdownTokenType.OrderedListItem => MarkdownStyles.Default.OrderedListItem,
        MarkdownTokenType.TableCell => MarkdownStyles.Default.TableCell,
        MarkdownTokenType.Emphasis => MarkdownStyles.Default.Emphasis,
        MarkdownTokenType.TypographicReplacement => MarkdownStyles.Default.TypographicReplacement,
        MarkdownTokenType.FootnoteReference => MarkdownStyles.Default.FootnoteReference,
        MarkdownTokenType.FootnoteDefinition => MarkdownStyles.Default.FootnoteDefinition,
        MarkdownTokenType.DefinitionTerm => MarkdownStyles.Default.DefinitionTerm,
        MarkdownTokenType.DefinitionDescription => MarkdownStyles.Default.DefinitionDescription,
        MarkdownTokenType.Abbreviation => MarkdownStyles.Default.Abbreviation,
        MarkdownTokenType.CustomContainer => MarkdownStyles.Default.CustomContainer,
        MarkdownTokenType.HtmlTag => MarkdownStyles.Default.HtmlTag,
        MarkdownTokenType.Subscript => MarkdownStyles.Default.Subscript,
        MarkdownTokenType.Superscript => MarkdownStyles.Default.Superscript,
        MarkdownTokenType.InsertedText => MarkdownStyles.Default.InsertedText,
        MarkdownTokenType.MarkedText => MarkdownStyles.Default.MarkedText,
        MarkdownTokenType.Emoji => MarkdownStyles.Default.Emoji,
        _ => MarkdownStyles.Default.DefaultStyle
    };

    internal static MarkdownInlineWriter Create(IAnsiConsole ansiConsole) => new(ansiConsole);
}

