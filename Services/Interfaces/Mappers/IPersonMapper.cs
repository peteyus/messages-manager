namespace Services.Interfaces.Mappers
{
    using Core.Interfaces;
    public interface IPersonMapper : IModelMapper<Core.Models.Person, Data.Models.Person>, IModelMapper<Data.Models.Person, Core.Models.Person>
    {
    }
}
