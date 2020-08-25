using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WithBackendBot.Cosmos
{
    public class CosmosService : ICosmosService
    {
        private Container _container;

        public CosmosService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<Repository>> GetRepositoriesAsync(QueryDefinition queryDefinition)
        {
            var query = this._container.GetItemQueryIterator<Repository>(queryDefinition);
            List<Repository> results = new List<Repository>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }
    }
}
