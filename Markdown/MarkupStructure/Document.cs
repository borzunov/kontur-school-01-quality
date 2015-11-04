using System.Collections.Generic;

namespace Markdown.MarkupStructure
{
    public class Document
    {
        public readonly List<Paragraph> Paragraphs;

        public Document(List<Paragraph> paragraphs)
        {
            Paragraphs = paragraphs;
        }
    }
}
