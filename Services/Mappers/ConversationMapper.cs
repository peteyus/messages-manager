namespace Services.Mappers
{
    using Core.Extensions;
    using Core.Models;
    using Services.Data.Models.Equality;
    using Services.Interfaces.Mappers;

    using DataConversation = Services.Data.Models.Conversation;

    public class ConversationMapper : IConversationMapper
    {
        private readonly IPersonMapper personMapper;
        private readonly DataEqualityComparer comparer = new DataEqualityComparer();

        public ConversationMapper(IPersonMapper personMapper)
        {
            personMapper.ThrowIfNull(nameof(personMapper));
            this.personMapper = personMapper;
        }

        public DataConversation Map(Conversation input)
        {
            var result = new DataConversation();

            result.Id = input.Id;
            result.MessageCount = input.MessageCount;
            foreach (var person in input.People)
            {
                var mappedPerson = this.personMapper.Map(person);
                result.People.Add(mappedPerson);
            }

            return result;
        }

        public Conversation Map(DataConversation input)
        {
            var result = new Conversation();

            result.Id = input.Id;
            result.MessageCount = input.MessageCount;
            foreach (var person in input.People)
            {
                var mappedPerson = this.personMapper.Map(person);
                result.People.Add(mappedPerson);
            }

            return result;
        }
        
        public DataConversation UpdateConversation(DataConversation state, Conversation input)
        {
            state.ThrowIfNull(nameof(state));
            input.ThrowIfNull(nameof(input));

            foreach (var person in input.People)
            {
                var matched = state.People.FirstOrDefault(p => p.Id == person.Id);
                if (matched == null) // && person.Id > 0) TODO PRJ: If the person already exists, do we need different logic? I don't think so.
                {
                    state.People.Add(this.personMapper.Map(person));
                }
                else
                {
                    this.personMapper.UpdatePerson(matched, person);
                }
            }

            return state;
        }
    }
}
