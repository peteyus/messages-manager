namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IReactionMapper : IModelMapper<Core.Models.Conversation, Data.Models.Conversation>, IModelMapper<Data.Models.Conversation, Core.Models.Conversation>
    {
    }
}
