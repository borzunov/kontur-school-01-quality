using System.Text;

namespace Markdown.MarkupStructure
{
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
            return Equals((Text)obj);
        }

        public override int GetHashCode()
        {
            return (Content != null ? Content.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return $"Text(\"{Content}\")";
        }
    }
}
