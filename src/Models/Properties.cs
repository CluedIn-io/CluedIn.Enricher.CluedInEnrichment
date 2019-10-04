using System.Collections.Generic;
using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment.Models
{
	public class Properties
	{
		[JsonExtensionData]
		public IDictionary<string, object> Collection { get; set; } = new Dictionary<string, object>();
	}
}