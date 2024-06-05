using Core.Models;

namespace Core.Interfaces
{
    public interface IIndexer
    {
        /// <summary>
        /// Adds a <see cref="Message"/> to the index.
        /// </summary>
        /// <param name="message">The <see cref="Message"/> to add.</param>
        /// <param name="cancellationToken">The token to watch for cancellation.</param>
        /// <returns></returns>
        Task IndexMessage(Message message, CancellationToken cancellationToken);

        /// <summary>
        /// Searches the index for messages containing a mathch on <paramref name="searchString"/>
        /// </summary>
        /// <param name="searchString">The string value to search</param>
        /// <param name="cancellationToken">The token to watch for cancellation.</param>
        /// <returns></returns>
        Task<IEnumerable<Message>> SearchMessages(string searchString, CancellationToken cancellationToken);

        /// <summary>
        /// Creates the index if it doesn't already exist, leaves it be if it does.
        /// </summary>
        /// <param name="cancellationToken">The token to watch for cancellation.</param>
        /// <returns>An awaitable task</returns>
        Task InitializeIndex(CancellationToken cancellationToken);
    }
}
