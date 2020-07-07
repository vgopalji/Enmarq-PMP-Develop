using Newtonsoft.Json;

namespace CareStream.Models
{
    public class AzureADB2C
    {
        [JsonProperty(PropertyName = "TenantId")]
        public string TenantId { get; set; }

        [JsonProperty(PropertyName = "AppId")]
        public string AppId { get; set; }

        [JsonProperty(PropertyName = "ClientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty(PropertyName = "B2cExtensionAppClientId")]
        public string B2cExtensionAppClientId { get; set; }
        
        [JsonProperty(PropertyName = "ApplicationName")]
        public string ApplicationName { get; set; }
        [JsonProperty(PropertyName = "AppObjectId")]
        public string AppObjectId { get; set; }
        [JsonProperty(PropertyName = "Domain")]
        public string Domain { get; set; }

        [JsonProperty(PropertyName = "Instance")]
        public string Instance { get; set; }

        [JsonProperty(PropertyName = "GraphResource")]
        public string GraphResource { get; set; }

        [JsonProperty(PropertyName = "GraphResourceEndPoint")]
        public string GraphResourceEndPoint { get; set; }

        [JsonProperty(PropertyName = "AADGraphResourceId")]
        public string AADGraphResourceId { get; set; }

        [JsonProperty(PropertyName = "AADGraphVersion")]
        public string AADGraphVersion { get; set; }

        [JsonProperty(PropertyName = "ExtensionName")]
        public  string ExtensionName { get; set; }
    }
}
