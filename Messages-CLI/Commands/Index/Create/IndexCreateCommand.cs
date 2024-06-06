using Core.Interfaces;
using Elastic.Clients.Elasticsearch.Nodes;
using System.CommandLine;

namespace Messages.CLI.Commands.Index.Create
{
    public class IndexCreateCommand : CliCommand
    {
        const string name = "create";
        const string description = "Creates an index with a given name.";

        private readonly IIndexer _indexer;

        public IndexCreateCommand(IIndexer indexer) : base(name, description)
        {
            _indexer = indexer;

            this.SetHandler(this.ExecuteAsync);
        }

        public async Task ExecuteAsync()
        {
            await _indexer.InitializeIndex(CancellationToken.None);
        }
    }
}
