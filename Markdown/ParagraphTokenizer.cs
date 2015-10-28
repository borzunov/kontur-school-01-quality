using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    class ParagraphTokenizer
    {
        readonly Func<int>[] handlers;

        readonly string source;
        int index;

        List<IMarkupElement> elements;
        bool curEscaped, nextEscaped;

        IMarkupElement LastElement => elements.LastOrDefault();

        public ParagraphTokenizer(string source)
        {
            handlers = new Func<int>[]
            {
                HandleWhitespace, HandleMarkupTag, HandleEscapeChar, HandleTextCharacter
            };

            this.source = source;
        }

        int HandleWhitespace()
        {
            if (!char.IsWhiteSpace(source[index]))
                return 0;

            if (!(LastElement is Whitespace))
                elements.Add(new Whitespace());
            return 1;
        }

        int HandleMarkupTag()
        {
            if (curEscaped)
                return 0;
            foreach (var tag in MarkupSyntax.MarkupTags)
            {
                var mark = tag.Mark;
                if (index + mark.Length <= source.Length && source.Substring(index, mark.Length) == mark)
                {
                    elements.Add(new Tag(tag.Name, TagType.Undefined, mark));
                    return mark.Length;
                }
            }
            return 0;
        }

        int HandleEscapeChar()
        {
            if (source[index] != MarkupSyntax.EscapeChar || curEscaped)
                return 0;

            nextEscaped = true;
            return 1;
        }

        int HandleTextCharacter()
        {
            var text = LastElement as Text;
            if (text == null)
            {
                text = new Text();
                elements.Add(text);
            }
            text.Content.Append(source[index]);
            return 1;
        }

        public List<IMarkupElement> Tokenize()
        {
            elements = new List<IMarkupElement>();
            index = 0;
            while (index < source.Length)
            {
                curEscaped = nextEscaped;
                nextEscaped = false;

                var parsed = false;
                foreach (var curHandler in handlers)
                {
                    var parsedCharsCount = curHandler();
                    if (parsedCharsCount > 0)
                    {
                        index += parsedCharsCount;
                        parsed = true;
                        break;
                    }
                }
                if (!parsed)
                    throw new ArgumentException($"Can't parse string \"{source}\" on position {index}");
            }
            return elements;
        }
    }
}
