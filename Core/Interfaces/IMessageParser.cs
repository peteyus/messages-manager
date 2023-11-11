namespace Core.Interfaces
{
    using Core.Models;

    public interface IMessageParser
    {
        public IEnumerable<Message> ReadMessages(); // TODO PRJ: Source? Always assume file? Config object?
    }
}
