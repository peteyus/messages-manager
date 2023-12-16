namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IVideoMapper : IModelMapper<Core.Models.Video, Data.Models.Video>, IModelMapper<Data.Models.Video, Core.Models.Video>
    {
    }
}
