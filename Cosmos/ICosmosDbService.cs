using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WithBackendBot.Cosmos
{
    public interface ICosmosService
    {
        Task<IEnumerable<Repository>> GetRepositoriesAsync(QueryDefinition queryDefinition);
    }
}