namespace Core.Models.Equality
{
    using System.Diagnostics.CodeAnalysis;

    public class MessageEqualityComparer : IEqualityComparer<Message>, IEqualityComparer<Person>, IEqualityComparer<Share>
    {
        public bool Equals(Message? x, Message? y)
        {
            if (x == null && y == null) return true;
            if (x== null || y == null) return false;

            // TODO
            var propertiesMatch = x.MessageText == y.MessageText;
            propertiesMatch &= x.Timestamp == y.Timestamp;
            propertiesMatch &= Equals(x.Sender, y.Sender);
            propertiesMatch &= Equals(x.Share, y.Share);

            return propertiesMatch;
        }

        public bool Equals(Person? x, Person? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            // TODO
            var propertiesMatch = x.Name == y.Name;
            propertiesMatch &= x.DisplayName == y.DisplayName;
            propertiesMatch &= x.ImageUrl == y.ImageUrl;
            propertiesMatch &= x.ThumbnailUrl == y.ThumbnailUrl;

            return propertiesMatch;
        }

        public bool Equals(Share? x, Share? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            // TODO
            var propertiesMatch = x.ShareText == y.ShareText;
            propertiesMatch &= x.OriginalContentOwner == y.OriginalContentOwner;
            propertiesMatch &= x.Url == y.Url;

            return propertiesMatch;
        }

        public int GetHashCode([DisallowNull] Message obj)
        {
            unchecked
            {
                int hashCode = 137;

                hashCode = (hashCode * 317) ^ (obj.MessageText?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.Sender is null ? 0 : GetHashCode(obj.Sender));
                hashCode = (hashCode * 317) ^ (obj.Timestamp.GetHashCode());
                hashCode = (hashCode * 317) ^ (obj.Share is null ? 0 : GetHashCode(obj.Share));

                return hashCode;
            }
        }

        public int GetHashCode([DisallowNull] Person obj)
        {
            unchecked
            {
                int hashCode = 137;
                hashCode = (hashCode * 317) ^ (obj.Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.DisplayName?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.ThumbnailUrl?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.ImageUrl?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        public int GetHashCode([DisallowNull] Share obj)
        {
            unchecked
            {
                int hashCode = 137;
                hashCode = (hashCode * 317) ^ (obj.Url?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.OriginalContentOwner?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.ShareText?.GetHashCode() ?? 0);

                return hashCode;
            }
        }
    }
}
