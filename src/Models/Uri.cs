using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment.Models
{
	public class Uri
	{
		[JsonProperty("uri")]
		public string UriString { get; set; }

		[JsonProperty("origin")]
		public string Origin { get; set; }
	}
}