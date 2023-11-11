namespace Core.Interfaces
{
    using Core.Models;

    public interface IMessageRepository
    {
        IEnumerable<Message> GetMessages(); // Config object? Allow search/range/criteria in single method? Or Overrides?

        void ImportMessages(IMessageParser parser);
    }
}
