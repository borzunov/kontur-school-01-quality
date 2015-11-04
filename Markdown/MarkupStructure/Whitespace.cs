namespace Markdown.MarkupStructure
{
    public class Whitespace : IMarkupElement
    {
        protected bool Equals(Whitespace other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Whitespace)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "Whitespace()";
        }
    }
}
