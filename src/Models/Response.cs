using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment.Models
{
	public class Response
    {
        [JsonProperty("Metadata")]
        public Metadata Metadata { get; set; }
    }
}