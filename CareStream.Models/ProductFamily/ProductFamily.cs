using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CareStream.Models
{
   
    public class ProductFamilyModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyId", Required = Required.Default)]
        [Key]
        public string ProductFamilyId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
        public string ProductFamilyName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
        public string ProductDescription { get; set; }
        public List<AssignedDealerModel> assignedDealerModels { get; set; }
        [NotMapped]
        public virtual List<DealerModel> dealerModel { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> selectedDealer { get; set; }
        [NotMapped]
        public int dealerCount { get; set; }
    }

    public class AssignedProductFamilyModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyId", Required = Required.Default)]
        [Key]
        public string ProductFamilyId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
        public string ProductFamilyName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
        public string ProductDescription { get; set; }      

    }

    public class DeletedDealerProductFamilyModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyId", Required = Required.Default)]
        [Key]
        public string ProductFamilyId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
        public string ProductFamilyName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
        public string ProductDescription { get; set; }        

    }



    public class DeletedProductFamily
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyId", Required = Required.Default)]
        [Key]
        public string ProductFamilyId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
        public string ProductFamilyName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
        public string ProductDescription { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> selectedDealer { get; set; }
        public List<DeletedProductFamilyDealerModel> deletedProductFamilyDealerModels { get; set; }
        [NotMapped]
        public virtual List<DealerModel> dealerModels { get; set; }

    }


}
