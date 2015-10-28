using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Markdown
{
    [TestFixture]
    public class ParagraphTokenizer_Specification
    {
        [Test]
        public void tokenize_collapsesWhitespaceGroups()
        {
            var tokenizer = new ParagraphTokenizer("   some  different\nwhite\t spaces");

            var elements = tokenizer.Tokenize();

            CollectionAssert.AreEqual(new IMarkupElement []
            {
                new Whitespace(), new Text("some"),
                new Whitespace(), new Text("different"),
                new Whitespace(), new Text("white"),
                new Whitespace(), new Text("spaces"), 
            }, elements);
        }

        [Test]
        public void tokenize_handlesDifferentMarkupTags()
        {
            var tokenizer = new ParagraphTokenizer("_em_ __strong__ `code`");
            
            var elements = tokenizer.Tokenize();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Tag("em"), new Text("em"), new Tag("em"),
                new Whitespace(), 
                new Tag("strong"), new Text("strong"), new Tag("strong"),
                new Whitespace(), 
                new Tag("code"), new Text("code"), new Tag("code"),
            }, elements);
        }

        [Test]
        public void tokenize_doesntCareAboutTagBalancing()
        {
            var tokenizer = new ParagraphTokenizer("unbalanced _tag");

            var elements = tokenizer.Tokenize();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("unbalanced"), 
                new Whitespace(), 
                new Tag("em"), new Text("tag"),
            }, elements);
        }


        [Test]
        public void tokenize_doesntCareAboutTagPosition()
        {
            var tokenizer = new ParagraphTokenizer("inside__word evenWithDigits_123");

            var elements = tokenizer.Tokenize();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("inside"), new Tag("strong"), new Text("word"), 
                new Whitespace(),
                new Text("evenWithDigits"), new Tag("em"), new Text("123"), 
            }, elements);
        }

        [Test]
        public void tokenize_handlesEscapeSequences()
        {
            var tokenizer = new ParagraphTokenizer(@"\`\_\_markupChars \\backslashItself \preservesLetters");

            var elements = tokenizer.Tokenize();

            CollectionAssert.AreEqual(new IMarkupElement[]
            {
                new Text("`__markupChars"),
                new Whitespace(), 
                new Text(@"\backslashItself"),
                new Whitespace(),
                new Text(@"preservesLetters"),
            }, elements);
        }
    }
}