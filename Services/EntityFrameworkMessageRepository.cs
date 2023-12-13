namespace Services
{
    using Core.Interfaces;
    using Core.Models;

    public class EntityFrameworkMessageRepository : IMessageRepository
    {
        public Conversation GetConversation(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Conversation> GetConversations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> GetMessages()
        {
            throw new NotImplementedException();
        }

        public void ImportMessagesToConversation(Conversation conversation, IEnumerable<Message> messages)
        {
            throw new NotImplementedException();
        }

        public void SaveConversation(Conversation conversation)
        {
            throw new NotImplementedException();
        }
    }
}
