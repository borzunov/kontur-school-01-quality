using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdown
{
    class MarkupTag
    {
        public readonly string Name;
        public readonly string Mark;

        public MarkupTag(string name, string mark)
        {
            Name = name;
            Mark = mark;
        }
    }

    static class MarkupSyntax
    {
        public const char EscapeChar = '\\';

        public static readonly MarkupTag[] MarkupTags = new[]
        {
            new MarkupTag("code", "`"), 
            new MarkupTag("strong", "__"), 
            new MarkupTag("em", "_"), 
        };
    }
}
