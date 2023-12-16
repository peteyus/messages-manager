namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IPhotoMapper : IModelMapper<Core.Models.Photo, Data.Models.Photo>, IModelMapper<Data.Models.Photo, Core.Models.Photo>
    {
    }
}
