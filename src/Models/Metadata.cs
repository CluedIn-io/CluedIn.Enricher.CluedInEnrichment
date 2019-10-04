using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment.Models
{
	public class Metadata
	{
		[JsonProperty("processedData")]
		public ProcessedData ProcessedData { get; set; }
	}
}