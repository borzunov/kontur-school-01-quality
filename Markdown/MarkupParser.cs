using System.Collections.Generic;
using System.Linq;
using Markdown.MarkupStructure;

namespace Markdown
{
    class MarkupParser
    {
        const string CodeTagName = "code";

        readonly List<IMarkupElement> tokens;

        public MarkupParser(List<IMarkupElement> tokens)
        {
            this.tokens = tokens;
        }

        static List<IMarkupElement> StripWhitespaces(List<IMarkupElement> elements)
        {
            // CR (krait): Лишнее копирование списка, если первый элемент не Whitespace.
            var newElements = (elements[0] is Whitespace ? elements.Skip(1) : elements).ToList();
            if (newElements[newElements.Count - 1] is Whitespace)
                newElements.RemoveAt(newElements.Count - 1);
            return newElements;
        }

        static Text RestoreText(Tag tag)
        {
            return new Text(tag.RawText);
        }

        static bool IsThereWordBreak(IMarkupElement elem, bool atTheEnd)
        {
            if (elem is Whitespace)
                return true;
            var text = elem as Text;
            if (text != null && text.Content.Length >= 1)
            {
                var adjacentChar = text.Content[atTheEnd ? text.Content.Length - 1 : 0];
                return !char.IsLetterOrDigit(adjacentChar);
            }
            return false;
        }

        static bool TagCanBeAt(List<IMarkupElement> elements, int index, TagType tagType)
        {
            var paragraphBegin = (index == 0);
            var paragraphEnd = (index == elements.Count - 1);
            return (
                (
                    tagType == TagType.Opening &&
                    (paragraphBegin || IsThereWordBreak(elements[index - 1], true)) &&
                    (!paragraphEnd && !(elements[index + 1] is Tag))
                ) ||
                (
                    tagType == TagType.Closing &&
                    (!paragraphBegin && !(elements[index - 1] is Tag)) &&
                    (paragraphEnd || IsThereWordBreak(elements[index + 1], false))
                ));
        }

        static List<IMarkupElement> SetTagTypes(List<IMarkupElement> elements, string tagName,
            bool requireWordBorders)
        {
            var newElements = new List<IMarkupElement>();
            var tagOpened = false;
            var lastTagIndex = -1;
            for (var i = 0; i < elements.Count; i++)
            {
                var elem = elements[i];
                var tag = elem as Tag;
                if (tag != null && tag.Name == tagName)
                {
                    var newTagType = tagOpened ? TagType.Closing : TagType.Opening;
                    if (requireWordBorders && !TagCanBeAt(elements, i, newTagType))
                        newElements.Add(RestoreText(tag));
                    else
                    {
                        newElements.Add(new Tag(tag.Name, newTagType, tag.RawText));
                        tagOpened = !tagOpened;
                        lastTagIndex = i;
                    }
                    continue;
                }
                newElements.Add(elem);
            }
            if (tagOpened)
                newElements[lastTagIndex] = RestoreText((Tag)newElements[lastTagIndex]);
            return newElements;
        }

        static List<IMarkupElement> SkipTagsInCodeBlocks(List<IMarkupElement> elements)
        {
            var newElements = new List<IMarkupElement>();
            var codeBlockOpened = false;
            foreach (var elem in elements)
            {
                var tag = elem as Tag;
                if (tag != null)
                {
                    if (tag.Name == CodeTagName)
                        codeBlockOpened = !codeBlockOpened;
                    else if (codeBlockOpened)
                    {
                        newElements.Add(RestoreText(tag));
                        continue;
                    }
                }
                newElements.Add(elem);
            }
            return newElements;
        }

        static List<IMarkupElement> MergeAdjacentTextElements(List<IMarkupElement> elements)
        {
            var newElements = new List<IMarkupElement>();
            IMarkupElement lastElement = null;
            foreach (var elem in elements)
            {
                var prevText = lastElement as Text;
                var curText = elem as Text;
                if (prevText != null && curText != null)
                    prevText.Content.Append(curText.Content);
                else
                {
                    newElements.Add(elem);
                    lastElement = elem;
                }
            }
            return newElements;
        }

        public Paragraph Parse()
        {
            var elements = tokens;
            elements = StripWhitespaces(elements);
            elements = SetTagTypes(elements, CodeTagName, false);
            elements = SkipTagsInCodeBlocks(elements);
            foreach (var tag in MarkupSyntax.MarkupTags)
                if (tag.Name != CodeTagName)
                    elements = SetTagTypes(elements, tag.Name, true);
            elements = MergeAdjacentTextElements(elements);
            return new Paragraph(elements);
        }
    }
}
