using Newtonsoft.Json;

namespace WithBackendBot.Cosmos
{
    public class Repository
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string DBName { get; set; }
        public string InstanceName { get; set; }
        public string Status { get; set; }
        public string RDMSType { get; set; }
        public string Version { get; set; }
        public string HostName { get; set; }
        public string AppName { get; set; }
        public string Owner { get; set; }


    }
}
