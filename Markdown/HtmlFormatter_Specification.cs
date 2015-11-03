using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Markdown
{
    class HtmlFormatter_Specification
    {
        void CheckParagraphsPresence(string actualSource, params string[] paragraphsPatterns)
        {
            var summaryPattern = String.Join(@"\s*", paragraphsPatterns
                .Select(pattern => $@"<p>\s*{pattern}\s*</p>"));
            Assert.IsTrue(Regex.IsMatch(actualSource, summaryPattern));
        }

        [Test]
        public void formatDocument_rendersText()
        {
            var processor = new DocumentProcessor("just a text");

            var document = processor.Process();
            var htmlSource = new HtmlFormatter(document).FormatDocument();

            CheckParagraphsPresence(htmlSource, "just a text");
        }
        
        [Test]
        public void formatDocument_avoidsXSS()
        {
            var processor = new DocumentProcessor("<b>XSS Test</b>");

            var document = processor.Process();
            var htmlSource = new HtmlFormatter(document).FormatDocument();

            CheckParagraphsPresence(htmlSource, "&lt;b&gt;XSS Test&lt;/b&gt;");
                
        }

        [Test]
        public void formatDocument_rendersWhitespaces()
        {
            var processor = new DocumentProcessor("yet another  test\nwith \t whitespaces");

            var document = processor.Process();
            var htmlSource = new HtmlFormatter(document).FormatDocument();

            CheckParagraphsPresence(htmlSource, "yet another test with whitespaces");
        }

        [Test]
        public void formatDocument_rendersDifferentTags()
        {
            var processor = new DocumentProcessor("_em_ __strong__ `code` word_with_digits_123");

            var document = processor.Process();
            var htmlSource = new HtmlFormatter(document).FormatDocument();

            CheckParagraphsPresence(htmlSource,
                "<em>em</em> <strong>strong</strong> <code>code</code> word_with_digits_123");
        }


        [Test]
        public void formatDocument_rendersNestedTags()
        {
            var processor = new DocumentProcessor("_ __ `emStrongCode` __ _");

            var document = processor.Process();
            var htmlSource = new HtmlFormatter(document).FormatDocument();

            CheckParagraphsPresence(htmlSource,
                "<em> <strong> <code>emStrongCode</code> </strong> </em>");
        }

        [Test]
        public void formatDocument_rendersSeveralParagraphs()
        {
            var processor = new DocumentProcessor("\n\n\nthis text is\n\n divided to exactly two\n paragraphs");

            var document = processor.Process();
            var htmlSource = new HtmlFormatter(document).FormatDocument();

            CheckParagraphsPresence(htmlSource, "this text is", "divided to exactly two paragraphs");
        }
    }
}
