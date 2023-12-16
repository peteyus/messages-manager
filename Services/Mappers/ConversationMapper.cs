namespace Services.Mappers
{
    using Core.Extensions;
    using Services.Interfaces.Mappers;

    public class ConversationMapper : IConversationMapper
    {
        private readonly IMessageMapper messagesMapper;

        public ConversationMapper(IMessageMapper messagesMapper)
        {
            messagesMapper.ThrowIfNull(nameof(messagesMapper));
            this.messagesMapper = messagesMapper;
        }

        public Data.Models.Conversation Map(Core.Models.Conversation input)
        {
            throw new NotImplementedException();
        }

        public Core.Models.Conversation Map(Data.Models.Conversation input)
        {
            throw new NotImplementedException();
        }
    }
}
