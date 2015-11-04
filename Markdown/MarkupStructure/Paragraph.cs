using System.Collections.Generic;
using System.Linq;

namespace Markdown.MarkupStructure
{
    public class Paragraph
    {
        public readonly List<IMarkupElement> Elements;

        public Paragraph(List<IMarkupElement> elements)
        {
            Elements = elements;
        }

        protected bool Equals(Paragraph other)
        {
            return Elements.SequenceEqual(other.Elements);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Paragraph)obj);
        }

        public override int GetHashCode()
        {
            return (Elements != null ? Elements.GetHashCode() : 0);
        }
    }
}
