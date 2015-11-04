namespace Markdown.MarkupStructure
{
    public enum TagType { Undefined, Opening, Closing }

    public class Tag : IMarkupElement
    {
        public readonly string Name;
        public readonly TagType Type;
        public readonly string RawText;

        public Tag(string name, TagType type = TagType.Undefined, string rawText = null)
        {
            Name = name;
            Type = type;
            RawText = rawText;
        }

        protected bool Equals(Tag other)
        {
            return Name == other.Name && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Tag)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Type;
                hashCode = (hashCode*397) ^ (RawText != null ? RawText.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Tag(\"{Name}\", {Type})";
        }
    }
}
