namespace Services.Data.Models
{
    using Services.Data.Models.Equality;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Conversation
    {
        public Conversation()
        {
            this.Messages = new HashSet<Message>(new DataEqualityComparer());
            this.People = new HashSet<Person>(new DataEqualityComparer());
        }

        public int Id { get; set; }
        // TODO PRJ: My brain says we need this, maybe just for the count, but it feels like there's
        // something more. Leaving it for now, but remove if we can.
        public ICollection<Message> Messages { get; set; }
        [NotMapped]
        public int MessageCount { get; set; }
        public ICollection<Person> People { get; set; }
    }
}
