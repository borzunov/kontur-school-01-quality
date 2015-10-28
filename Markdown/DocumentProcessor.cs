using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Markdown
{
    class DocumentProcessor
    {
        static IEnumerable<string> SplitParagraphs(string source)
        {
            return Regex
                .Split(source, @"\n\s*\n")
                .Where(paragraphSource => paragraphSource != "");
        }

        readonly string source;

        public DocumentProcessor(string source)
        {
            this.source = source;
        }

        public Document Process()
        {
            var paragraphs = SplitParagraphs(source)
                .Select(paragraphSource =>
                {
                    var tokens = new ParagraphTokenizer(paragraphSource).Tokenize();
                    return new MarkupParser(tokens).Parse();
                })
                .ToList();
            return new Document(paragraphs);
        }
    }
}
