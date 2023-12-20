namespace Services.Mappers
{
    using Core.Extensions;
    using Core.Models;
    using Services.Interfaces.Mappers;

    using DataPerson = Services.Data.Models.Person;

    public class PersonMapper : IPersonMapper
    {
        public DataPerson Map(Person input)
        {
            var result = new DataPerson();
            result.Id = input.Id;
            result.Name = input.Name;
            result.DisplayName = input.DisplayName;
            result.ThumbnailUrl = input.ThumbnailUrl;
            result.ImageUrl = input.ImageUrl;

            return result;
        }

        public Person Map(DataPerson input)
        {
            var result = new Person();
            result.Id = input.Id;
            result.Name = input.Name;
            result.DisplayName = input.DisplayName;
            result.ThumbnailUrl = input.ThumbnailUrl;
            result.ImageUrl = input.ImageUrl;

            return result;
        }

        public DataPerson UpdatePerson(DataPerson state, Person input)
        {
            state.ThrowIfNull(nameof(state));
            input.ThrowIfNull(nameof(input));

            state.Name = input.Name;
            state.DisplayName = input.DisplayName;
            state.ThumbnailUrl = input.ThumbnailUrl;
            state.ImageUrl = input.ImageUrl;
            return state;
        }
    }
}
