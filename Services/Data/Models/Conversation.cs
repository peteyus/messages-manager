namespace Services.Data.Models
{
    using Services.Data.Models.Equality;
    using System.Collections.Generic;

    public class Conversation
    {
        public Conversation()
        {
            this.Messages = new HashSet<Message>(new DataEqualityComparer());
            this.People = new HashSet<Person>(new DataEqualityComparer());
        }

        public int Id { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Person> People { get; set; }
    }
}
