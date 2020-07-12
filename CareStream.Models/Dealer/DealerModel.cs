using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CareStream.Models
{
    public class DealerModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
        [Key]
        public string DealerId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
        public string DealerName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
        public string DealerDescription { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
        public string SAPID { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> selectedProductFamily { get; set; }
        //[JsonProperty(PropertyName = "productFamilyModels")]
        public List<AssignedProductFamilyModel> assignedProductFamilyModels { get; set; }
        [NotMapped]
        public virtual List<ProductFamilyModel> productFamilyModels { get; set; }
        [NotMapped]
        public int productFamilyCount { get; set; }

    }

    public class DeletedDealerModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
        [Key]
        public string DealerId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
        public string DealerName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
        public string DealerDescription { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
        public string SAPID { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> selectedProductFamily { get; set; }
        public List<DeletedDealerProductFamilyModel> deletedDealerProductFamilyModels { get; set; }
        [NotMapped]
        public virtual List<ProductFamilyModel> productFamilyModels { get; set; }

    }

    public class AssignedDealerModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
        [Key]        public string DealerId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
        public string DealerName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
        public string DealerDescription { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
        public string SAPID { get; set; }
    }

    public class DeletedProductFamilyDealerModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
        [Key] public string DealerId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
        public string DealerName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
        public string DealerDescription { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
        public string SAPID { get; set; }

    }

}
