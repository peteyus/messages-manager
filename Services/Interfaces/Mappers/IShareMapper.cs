namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IShareMapper : IModelMapper<Core.Models.Share, Data.Models.Share>, IModelMapper<Data.Models.Share, Core.Models.Share>
    {
    }
}
