using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Markdown
{
    class MarkupParser_Specification
    {
        [Test]
        public void parse_stripsWhitespacesOnSides()
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
        public void parse_setsTagTypes()
        {
            var tokenizer = new ParagraphTokenizer("_em_ __strong__ `code`");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("em", TagType.Opening), new Text("em"), new Tag("em", TagType.Closing),
                new Whitespace(),
                new Tag("strong", TagType.Opening), new Text("strong"), new Tag("strong", TagType.Closing),
                new Whitespace(),
                new Tag("code", TagType.Opening), new Text("code"), new Tag("code", TagType.Closing),
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
        public void parse_allowsToUseCodeTagsEverywhere()
        {
            var tokenizer = new ParagraphTokenizer("`some code` inside`word` evenWith`digit`123");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("code", TagType.Opening),
                    new Text("some"), new Whitespace(), new Text("code"),
                new Tag("code", TagType.Closing),
                new Whitespace(),
                new Text("inside"),
                new Tag("code", TagType.Opening), new Text("word"), new Tag("code", TagType.Closing),
                new Whitespace(),
                new Text("evenWith"),
                new Tag("code", TagType.Opening), new Text("digit"), new Tag("code", TagType.Closing),
                new Text("123"),
            }, paragraph.Elements);
        }


        [Test]
        public void parse_allowsToUseOtherFormattingTagsOnlyOnWordBorders()
        {
            var tokenizer = new ParagraphTokenizer(
                "text_with_numbers_123 text__just__with_underscores " +
                "__formattingShould lookLikeThis__ " +
                "_underscoresCanOccur _ in_formatting_");

            var elements = tokenizer.Tokenize();
            var paragraph = new MarkupParser(elements).Parse();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("text_with_numbers_123"), new Whitespace(),
                new Text("text__just__with_underscores"), new Whitespace(),
                new Tag("strong", TagType.Opening),
                    new Text("formattingShould"), new Whitespace(),
                    new Text("lookLikeThis"),
                new Tag("strong", TagType.Closing),
                new Whitespace(), 
                new Tag("em", TagType.Opening),
                    new Text("underscoresCanOccur"), new Whitespace(),
                    new Text("_"), new Whitespace(),
                    new Text("in_formatting"),
                new Tag("em", TagType.Closing),
            }, paragraph.Elements);
        }

        [Test]
        public void parse_allowsNestedTags()
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
        public void parse_insideCodeBlocks_prohibitsFormatting()
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
        public void parse_insideCodeBlocks_collapsesWhitespaceGroups()
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
