namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IConversationMapper : IModelMapper<Core.Models.Conversation, Data.Models.Conversation>, IModelMapper<Data.Models.Conversation, Core.Models.Conversation>
    {
        public Data.Models.Conversation UpdateConversation(Data.Models.Conversation state, Core.Models.Conversation input);
    }
}
