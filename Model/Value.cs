using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WithBackendBot.Cosmos;

namespace WithBackendBot.Model
{
    public class Value
    {
        public IEnumerable<Repository> reposataries { get; set; }
    }
}
