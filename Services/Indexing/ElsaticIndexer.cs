using Core.Interfaces;
using Core.Models;
using Core.Models.Application;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Services.Indexing
{
    public class ElsaticIndexer : IIndexer
    {
        public ElsaticIndexer(ApplicationConfiguration config)
        {
            if (config?.Elastic?.ApiUrl == null || config?.Elastic?.ApiKey == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var client = new ElasticsearchClient(config.Elastic.ApiUrl, new ApiKey(config.Elastic.ApiKey));
        }
        public void IndexMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Message> SearchMessages(string searchString)
        {
            throw new NotImplementedException();
        }
    }
}
