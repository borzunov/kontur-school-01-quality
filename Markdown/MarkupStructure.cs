using System;
using System.Text;

namespace Markdown
{
    public interface IMarkupElement { }

    public class Text : IMarkupElement
    {
        public readonly StringBuilder Content;

        public Text()
        {
            Content = new StringBuilder();
        }

        public Text(string content)
        {
            Content = new StringBuilder(content);
        }

        protected bool Equals(Text other)
        {
            return Content.ToString() == other.Content.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Text) obj);
        }

        public override string ToString()
        {
            return $"Text(\"{Content}\")";
        }
    }

    public class Whitespace : IMarkupElement {
        protected bool Equals(Whitespace other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Whitespace) obj);
        }

        public override string ToString()
        {
            return "Whitespace()";
        }
    }

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
            return Equals((Tag) obj);
        }

        public override string ToString()
        {
            return $"Tag(\"{Name}\", {Type})";
        }
    }

    public class Paragraph
    {
        public readonly IMarkupElement[] Elements;

        public Paragraph(IMarkupElement[] elements)
        {
            Elements = elements;
        }
    }
}