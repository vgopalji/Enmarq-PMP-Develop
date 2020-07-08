using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        //public List<ProductFamilyCosmos> ProductFamilyCosmos { get; set; }
        //public List<DealerModelCosmos> DealerModelCosmos { get; set; }
        //public DealerModel dealerModel { get; set; }

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
        //public List<ProductFamilyCosmos> ProductFamilyCosmos { get; set; }
        //public List<DealerModelCosmos> DealerModelCosmos { get; set; }
        //public DealerModel dealerModel { get; set; }

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



    //public class ProductFamilyModel
    //{
    //    public ProductFamilyModel()
    //    {
    //        dealerAssignModel = new DealerAssignModel();
    //        DealerSelected = new List<string>();
    //    }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
    //    public string ProductFamilyName { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
    //    public string ProductDescription { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerAssignModel1", Required = Required.Default)]
    //    public DealerAssignModel dealerAssignModel { get; set; }

    //    [JsonProperty(PropertyName = "dealerselected", NullValueHandling = NullValueHandling.Ignore)]
    //    public List<string> DealerSelected { get; set; }

    //}

    //public class ProductFamilyCosmos
    //{
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyId", Required = Required.Default)]
    //    public string ProductFamilyId { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
    //    public string ProductFamilyName { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
    //    public string ProductDescription { get; set; }
    //    public List<DealerModelCosmos> dealerModelCosmos { get; set; }
    //}
    //public class DealerModelCosmos
    //{
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
    //    public string DealerId { get; set; }

    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
    //    public string DealerName { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
    //    public string DealerDescription { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
    //    public string SAPID { get; set; }

    //}

    //public class ProductFamilyViewModel1
    //{
    //    //public List<ProductFamilyCosmos> ProductFamilyCosmos { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyId", Required = Required.Default)]
    //    public string ProductFamilyId { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productFamilyName", Required = Required.Default)]
    //    public string ProductFamilyName { get; set; }
    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "productDescription", Required = Required.Default)]
    //    public string ProductDescription { get; set; }
    //    public List<DealerModelCosmos> DealerModelCosmos { get; set; }
    //}
    ////public class DealerAssignModel1
    ////{
    ////    public DealerAssignModel1()
    ////    {
    ////        DealerList1 = new Dictionary<string, string>();
    ////    }

    ////    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assignfor", Required = Required.Default)]
    ////    public string AssignFor { get; set; }

    ////    [JsonProperty(PropertyName = "dealerList", NullValueHandling = NullValueHandling.Ignore)]
    ////    public Dictionary<string, string> DealerList1 { get; set; }


    ////}


}
