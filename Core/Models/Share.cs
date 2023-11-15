namespace Core.Models
{
    using System.Diagnostics.CodeAnalysis;

    public class Share : IEquatable<Share>, IEqualityComparer<Share>
    {
        public string? Url { get; set; }
        public string? ShareText { get; set; }
        public string? OriginalContentOwner { get; set; }

        public bool Equals(Share? other)
        {
            return this.Equals(this, other);
        }

        public bool Equals(Share? x, Share? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Url == y.Url && x.ShareText == y.ShareText && x.OriginalContentOwner == y.OriginalContentOwner;
        }

        public int GetHashCode([DisallowNull] Share obj)
        {
            return (obj.Url, obj.ShareText, obj.OriginalContentOwner).GetHashCode();
        }
    }
}
