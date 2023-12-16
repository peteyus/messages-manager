namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IMessageMapper : IModelMapper<Core.Models.Message, Data.Models.Message>, IModelMapper<Data.Models.Message, Core.Models.Message>
    {
    }
}
