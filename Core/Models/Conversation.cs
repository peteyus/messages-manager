namespace Core.Models
{
    public class Conversation
    {
        public Conversation()
        {
            this.People = new List<Person>();
        }

        public int Id { get; set; }
        public int MessageCount { get; set; }
        public IList<Person> People { get; set; }
    }
}
