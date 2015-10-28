using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Markdown
{
    class MarkupParser_Specification
    {
        [Test]
        public void parse_removesLeadingAndTrailingWhitespaces()
        {
            var tokenizer = new ParagraphTokenizer("   several  words ");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("several"), new Whitespace(), new Text("words"),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_discoversStructureOfDifferentTags()
        {
            var tokenizer = new ParagraphTokenizer("_emphasizedText_ __strongText__ `codeText`");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("em", TagType.Opening),
                    new Text("emphasizedText"),
                new Tag("em", TagType.Closing),
                new Whitespace(),
                new Tag("strong", TagType.Opening),
                    new Text("strongText"),
                new Tag("strong", TagType.Closing),
                new Whitespace(),
                new Tag("code", TagType.Opening),
                    new Text("codeText"),
                new Tag("code", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_transformsUnbalancedTagsBackToText()
        {
            var tokenizer = new ParagraphTokenizer("unbalanced _tag");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("unbalanced"), new Whitespace(), new Text("_tag"),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesCodeBlocks_everywhere()
        {
            var tokenizer = new ParagraphTokenizer("`justCode` orCodeInsideWordWith`digits`123");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("code", TagType.Opening),
                    new Text("justCode"),
                new Tag("code", TagType.Closing),
                new Whitespace(),
                new Text("orCodeInsideWordWith"),
                new Tag("code", TagType.Opening),
                    new Text("digits"),
                new Tag("code", TagType.Closing),
                new Text("123"),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesEmptyCodeBlocks()
        {
            var tokenizer = new ParagraphTokenizer("emptyCodeBlock: ``");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("emptyCodeBlock:"), 
                new Whitespace(), 
                new Tag("code", TagType.Opening),
                new Tag("code", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesOtherTags_atWordBordersNearWhitespace()
        {
            var tokenizer = new ParagraphTokenizer(
                "FormattingCan __startAtAWordBeginning afterAWhitespace and " +
                "endAtAWordEnd__ beforeAWhitespace.");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("FormattingCan"),
                new Whitespace(),
                new Tag("strong", TagType.Opening),
                    new Text("startAtAWordBeginning"),
                    new Whitespace(),
                    new Text("afterAWhitespace"),
                    new Whitespace(),
                    new Text("and"),
                    new Whitespace(),
                    new Text("endAtAWordEnd"),
                new Tag("strong", TagType.Closing),
                new Whitespace(),
                new Text("beforeAWhitespace."),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesOtherTags_atWordBordersNearPunctuation()
        {
            var tokenizer = new ParagraphTokenizer(
                "*__FormattingAlsoCan startAfterAPunctuation and endBeforeAPunctuation__.");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("*"), 
                new Tag("strong", TagType.Opening),
                    new Text("FormattingAlsoCan"),
                    new Whitespace(), 
                    new Text("startAfterAPunctuation"),
                    new Whitespace(),
                    new Text("and"),
                    new Whitespace(),
                    new Text("endBeforeAPunctuation"), 
                new Tag("strong", TagType.Closing),
                new Text("."),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesOtherTags_atWordBordersNearDocumentBeginningAndEnd()
        {
            var tokenizer = new ParagraphTokenizer(
                "__FormattingAlsoCan startAtTheDocumentBeginning__ and __endAtTheEnd.__");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("strong", TagType.Opening),
                    new Text("FormattingAlsoCan"),
                    new Whitespace(),
                    new Text("startAtTheDocumentBeginning"),
                new Tag("strong", TagType.Closing),
                new Whitespace(),
                new Text("and"),
                new Whitespace(),
                new Tag("strong", TagType.Opening),
                    new Text("endAtTheEnd."),
                new Tag("strong", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_ignoresOtherTags_insideWordsFromLettersAndDigits()
        {
            var tokenizer = new ParagraphTokenizer(
                "text_with_numbers_123 text__just__with_underscores");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("text_with_numbers_123"), new Whitespace(),
                new Text("text__just__with_underscores")
            }, paragraph.Elements);
        }

        [Test]
        public void parse_ignoresOtherTags_insideCodeBlocks()
        {
            var tokenizer = new ParagraphTokenizer("`_trying_ __to format__`");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("code", TagType.Opening),
                    new Text("_trying_"), new Whitespace(),
                    new Text("__to"), new Whitespace(),
                    new Text("format__"),
                new Tag("code", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesNestedTags()
        {
            var tokenizer = new ParagraphTokenizer("_emphasized __strong `code` placed__ here_");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("em", TagType.Opening),
                    new Text("emphasized"),
                    new Whitespace(), 
                    new Tag("strong", TagType.Opening), 
                        new Text("strong"),
                        new Whitespace(),
                        new Tag("code", TagType.Opening), 
                            new Text("code"),
                        new Tag("code", TagType.Closing),
                        new Whitespace(),
                        new Text("placed"), 
                    new Tag("strong", TagType.Closing),
                    new Whitespace(),
                    new Text("here"), 
                new Tag("em", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_handlesAdjacentTagsSeparatedBySpace()
        {
            var tokenizer = new ParagraphTokenizer("_ __ `strongCode` __ _ `codeText`");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("em", TagType.Opening), 
                    new Whitespace(), 
                    new Tag("strong", TagType.Opening), 
                        new Whitespace(), 
                        new Tag("code", TagType.Opening),
                            new Text("strongCode"),
                        new Tag("code", TagType.Closing),
                        new Whitespace(), 
                    new Tag("strong", TagType.Closing), 
                    new Whitespace(), 
                new Tag("em", TagType.Closing), 
                new Whitespace(),
                new Tag("code", TagType.Opening),
                    new Text("codeText"),
                new Tag("code", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_ignoresDirectlyAdjacentNestedTags()
        {
            var tokenizer = new ParagraphTokenizer("_`notEmCode`_");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("_"), 
                new Tag("code", TagType.Opening),
                    new Text("notEmCode"), 
                new Tag("code", TagType.Closing),
                new Text("_"),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_collapsesWhitespaceGroups_evenInsideCodeBlocks()
        {
            var tokenizer = new ParagraphTokenizer("`  many\t\nwhitespaces\t\t`");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("code", TagType.Opening),
                    new Whitespace(), new Text("many"),
                    new Whitespace(), new Text("whitespaces"),
                    new Whitespace(),
                new Tag("code", TagType.Closing),
            }, paragraph.Elements);
        }
    }
}
