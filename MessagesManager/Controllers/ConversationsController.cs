namespace MessagesManager.Controllers
{
    using Core.Extensions;
    using Core.Interfaces;
    using Core.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IMessageRepository repository;

        public ConversationsController(IMessageRepository repository)
        {
            repository.ThrowIfNull(nameof(repository));
            this.repository = repository;
        }

        [HttpGet]
        public IEnumerable<Conversation> Get()
        {
            return this.repository.GetConversations();
        }
    }
}
