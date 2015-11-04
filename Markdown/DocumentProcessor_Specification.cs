using System.Collections.Generic;
using NUnit.Framework;
using Markdown.MarkupStructure;
using Text = Markdown.MarkupStructure.Text;

namespace Markdown
{
    class DocumentProcessor_Specification
    {
        [Test]
        public void process_splitsByTwoNewLines_toNonEmptyParagraphs()
        {
            var processor = new DocumentProcessor(
                "\n\nfirstPar" +
                "\n\nmiddlePar line1\n line2" +
                "\n\n\nlastPar");

            var document = processor.Process();

            CollectionAssert.AreEqual(new []
            {
                new Paragraph(new List<IMarkupElement> { new Text("firstPar") }),
                new Paragraph(new List<IMarkupElement>
                {
                    new Text("middlePar"), new Whitespace(),
                    new Text("line1"), new Whitespace(), new Text("line2")
                }),
                new Paragraph(new List<IMarkupElement> { new Text("lastPar") }),
            }, document.Paragraphs);
        }
    }
}
