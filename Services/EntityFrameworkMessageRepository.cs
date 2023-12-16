namespace Services
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using Services.Interfaces;

    public class EntityFrameworkMessageRepository : IMessageRepository
    {
        private readonly IMessageContext context;

        public EntityFrameworkMessageRepository(IMessageContext context)
        {
            context.ThrowIfNull(nameof(context));

            this.context = context;
        }

        public Conversation GetConversation(int id)
        {
            var result = this.context.Conversations.FirstOrDefault(conversation => conversation.Id == id);
            if (result == null)
            {
                throw new InvalidOperationException($"Conversastion ID {id} not found.");
            }

            return result;
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
