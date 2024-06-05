using Core.Models;

namespace Core.Interfaces
{
    public interface IIndexer
    {
        void IndexMessage(Message message);
        IEnumerable<Message> SearchMessages(string searchString);
    }
}
