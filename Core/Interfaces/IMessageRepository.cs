namespace Core.Interfaces
{
    using Core.Models;

    public interface IMessageRepository
    {
        IEnumerable<Conversation> GetConversations();
        IEnumerable<Message> GetMessages(); // Config object? Allow search/range/criteria in single method? Or Overrides?

        /// <summary>
        /// Retreives a <see cref="Conversation"/> from the data store by its ID
        /// </summary>
        /// <param name="id">The ID to search for</param>
        /// <returns><see cref="Conversation"/>. Throws a <see cref="InvalidOperationException"/> when the ID is not found.</returns>
        Conversation GetConversation(int id);

        /// <summary>
        /// Attaches the incoming <paramref name="messages"/> to the provided <paramref name="conversation"/>. 
        /// Does not save to the data store but does de-duplicate any messages that may already be stored in the data store.
        /// </summary>
        /// <param name="conversation">The <see cref="Conversation"/> to which the messages will be attached</param>
        /// <param name="messages">The <see cref="Message"/> objects to add to the given conversation</param>
        void ImportMessagesToConversation(Conversation conversation, IEnumerable<Message> messages);

        /// <summary>
        /// Takes the given <paramref name="conversation"/> and saves/updates it in the data store.
        /// </summary>
        /// <param name="conversation">The <see cref="Conversation"/> to save</param>
        void SaveConversation(Conversation conversation);
    }
}
