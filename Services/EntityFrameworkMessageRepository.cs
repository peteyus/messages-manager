namespace Services
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using Microsoft.EntityFrameworkCore;
    using Services.Interfaces;
    using Services.Interfaces.Mappers;

    public class EntityFrameworkMessageRepository : IMessageRepository
    {
        private readonly IMessageContext context;
        private readonly IConversationMapper conversationMapper;

        public EntityFrameworkMessageRepository(IMessageContext context,
            IConversationMapper conversationMapper)
        {
            context.ThrowIfNull(nameof(context));
            conversationMapper.ThrowIfNull(nameof(conversationMapper));

            this.context = context;
            this.conversationMapper = conversationMapper;
        }

        public Conversation GetConversation(int id)
        {
            var result = this.context.Conversations
                .Include(conversation => conversation.People).SingleOrDefault(c => c.Id == id);
            if (result == null)
            {
                throw new InvalidOperationException($"Conversastion ID {id} not found.");
            }

            int messageCount = this.context.Conversations.Select(c => new { c.Id, Count = c.Messages.Count() }).Single(c => c.Id == id).Count;
            result.MessageCount = messageCount;

            return this.conversationMapper.Map(result);
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
            conversation.ThrowIfNull(nameof(conversation));

            if (conversation.Id < 1)
            {
                this.SaveConversation(conversation);
            }

            foreach (var message in messages)
            {
                message.ConversationId = conversation.Id;
            }
            throw new NotImplementedException();
        }

        public void SaveConversation(Conversation conversation)
        {
            if (conversation.Id > 0)
            {
                var dataConversation = this.context.Conversations.Include(c => c.People).SingleOrDefault(c => c.Id == conversation.Id);
                dataConversation.ThrowIfNull(nameof(dataConversation)); // TODO PRJ: Better error here.
                this.conversationMapper.UpdateConversation(dataConversation, conversation);
            }
            else
            {
                var newConversation = this.conversationMapper.Map(conversation);
                this.context.Conversations.Add(newConversation);
            }
            throw new NotImplementedException();
        }
    }
}
