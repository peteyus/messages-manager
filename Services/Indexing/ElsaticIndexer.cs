using Core.Interfaces;
using Core.Models;
using Core.Models.Application;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Services.Indexing
{
    public class ElsaticIndexer : IIndexer
    {
        private readonly ElasticsearchClient client;
        private readonly ApplicationConfiguration config;

        public ElsaticIndexer(ApplicationConfiguration config)
        {
            this.config = config;

            // TODO PRJ: Build this earlier?
            var searchConfig = new ElasticsearchClientSettings(new Uri(config.Elastic.ApiUrl))
                .Authentication(new ApiKey(config.Elastic.ApiKey))
                .DefaultIndex(config.Elastic.IndexName);

            this.client = new ElasticsearchClient(searchConfig);
        }

        public async Task IndexMessage(Message message, CancellationToken cancellationToken)
        {
            await this.client.IndexAsync(message, cancellationToken);
        }

        public async Task<IEnumerable<Message>> SearchMessages(string searchString, CancellationToken cancellationToken)
        {
            var response = await this.client.SearchAsync<Message>(search => search
            .Index(config.Elastic.IndexName)
            .From(0)
            .Size(10) // TODO PRJ: Configurable? Pass in? Think about this one.
            .Query(query => query
                .Match(match => match.Query(searchString))
             ),
             cancellationToken);

            return response.Documents;
        }

        public async Task InitializeIndex(CancellationToken cancellationToken)
        {
            var indexExists = await this.client.Indices.ExistsAsync(config.Elastic.IndexName, cancellationToken);
            if (!indexExists.Exists)
            {
                await this.client.Indices.CreateAsync<Message>(config.Elastic.IndexName, cancellationToken);
            }
        }
    }
}
