namespace Core.Models
{
    public class Conversation
    {
        public Conversation()
        {
            this.Messages = new List<Message>();
            this.People = new List<Person>();
        }

        public int Id { get; set; }
        public List<Message> Messages { get; set; }
        public IList<Person> People { get; set; }
    }
}
