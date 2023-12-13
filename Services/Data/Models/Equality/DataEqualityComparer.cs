namespace Services.Data.Models.Equality
{
    using System.Diagnostics.CodeAnalysis;

    public class DataEqualityComparer : IEqualityComparer<Message>, IEqualityComparer<Person>, IEqualityComparer<Share>, IEqualityComparer<Video>, IEqualityComparer<Audio>, IEqualityComparer<Photo>, IEqualityComparer<MessageReaction>
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
            propertiesMatch &= Equals(x.Source, y.Source);

            var collectionsMatch = true;
            collectionsMatch &= !x.Videos.Except(y.Videos, this).Any();
            collectionsMatch &= !x.Images.Except(y.Images, this).Any();
            collectionsMatch &= !x.Audio.Except(y.Audio, this).Any();
            collectionsMatch &= !x.Links.Except(y.Links).Any();

            return propertiesMatch && collectionsMatch;
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

            var propertiesMatch = x.ShareText == y.ShareText;
            propertiesMatch &= x.OriginalContentOwner == y.OriginalContentOwner;
            propertiesMatch &= x.Url == y.Url;

            return propertiesMatch;
        }

        public bool Equals(Video? x, Video? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            return x.VideoUrl == y.VideoUrl;
        }

        public bool Equals(Audio? x, Audio? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            return x.FileUrl == y.FileUrl &&
                x.CreatedAt == y.CreatedAt;
        }

        public bool Equals(Photo? x, Photo? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            return x.ImageUrl == y.ImageUrl &&
                x.CreatedAt == y.CreatedAt;
        }

        public bool Equals(MessageReaction? x, MessageReaction? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            var propertiesMatch = x.Id == y.Id;
            propertiesMatch &= x.Reaction == y.Reaction;
            propertiesMatch &= Equals(x.Person, y.Person);

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
                hashCode = (hashCode * 317) ^ (obj.Source?.GetHashCode() ?? 0);

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

        public int GetHashCode([DisallowNull] Video obj)
        {
            unchecked
            {
                int hashCode = 137;
                hashCode = (hashCode * 317) ^ (obj.VideoUrl?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        public int GetHashCode([DisallowNull] Audio obj)
        {
            unchecked
            {
                int hashCode = 137;
                hashCode = (hashCode * 317) ^ (obj.FileUrl?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.CreatedAt?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        public int GetHashCode([DisallowNull] Photo obj)
        {
            unchecked
            {
                int hashCode = 137;
                hashCode = (hashCode * 317) ^ (obj.ImageUrl?.GetHashCode() ?? 0);
                hashCode = (hashCode * 317) ^ (obj.CreatedAt?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        public int GetHashCode([DisallowNull] MessageReaction obj)
        {
            throw new NotImplementedException();
        }
    }
}
