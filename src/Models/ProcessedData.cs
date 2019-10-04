using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment.Models
{
	public class ProcessedData
	{
		[JsonProperty("entityType")]
		public string EntityType { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("aliases")]
		public List<string> Aliases { get; set; }

		[JsonProperty("provider-origins")]
		public List<string> ProviderOrigins { get; set; }

		[JsonProperty("uri")]
		public string Uri { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("externalReferences")]
		public List<string> ExternalReferences { get; set; }

		[JsonProperty("properties")]
		public Properties Properties { get; set; }

		[JsonProperty("timeToLive")]
		public string TimeToLive { get; set; }

		[JsonProperty("isShadowEntity")]
		public string IsShadowEntity { get; set; }

		[JsonProperty("uris")]
		public List<Uri> Uris { get; set; }
	}
}