namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IAudioMapper : IModelMapper<Core.Models.Audio, Data.Models.Audio>, IModelMapper<Data.Models.Audio, Core.Models.Audio>
    {
    }
}
