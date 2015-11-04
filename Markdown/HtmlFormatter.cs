using System;
using System.Linq;
using System.Security;
using Markdown.MarkupStructure;

namespace Markdown
{
    class HtmlFormatter
    {
        readonly Document document;

        public HtmlFormatter(Document document)
        {
            this.document = document;
        }

        static string FormatElement(IMarkupElement elem)
        {
            var text = elem as Text;
            if (text != null)
                return SecurityElement.Escape(text.Content.ToString());

            if (elem is Whitespace)
                return " ";

            var tag = elem as Tag;
            if (tag != null)
            {
                switch (tag.Type)
                {
                    case TagType.Opening:
                        return $"<{tag.Name}>";
                    case TagType.Closing:
                        return $"</{tag.Name}>";
                    default:
                        throw new ArgumentException($"Can't format tag with type {tag.Type}");
                }
            }

            throw new ArgumentException($"Unknown markup element {elem}");
        }

        static string FormatParagraph(Paragraph paragraph)
        {
            var content = string.Join("", paragraph.Elements.Select(FormatElement));
            return $"<p>\n    {content}\n</p>";
        }

        const string DocumentHeader = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"" />
    <title>Converted Document</title>
</head>
<body>";
        const string DocumentFooter = @"</body>
</html>";

        public string FormatDocument()
        {
            var content = string.Join("\n", document.Paragraphs.Select(FormatParagraph));
            return $"{DocumentHeader}\n{content}\n{DocumentFooter}\n";
        }
    }
}
