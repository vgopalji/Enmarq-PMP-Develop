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

    //    public class DealerAssignModel
    //    {
    //        public DealerAssignModel()
    //        {
    //            DealerList = new Dictionary<string, string>();
    //        }

    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assignfor", Required = Required.Default)]
    //        public string AssignFor { get; set; }

    //        [JsonProperty(PropertyName = "dealerList", NullValueHandling = NullValueHandling.Ignore)]
    //        public Dictionary<string, string> DealerList { get; set; }

    //    }

    //    public class DealerDeleted
    //    {

    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
    //        public string DealerId { get; set; }

    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
    //        public string DealerName { get; set; }
    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
    //        public string DealerDescription { get; set; }
    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
    //        public string SAPID { get; set; }
    //    }




    //    public class DealerModelVm
    //    {
    //        public DealerModelVm()
    //        {
    //            ProductFamilyAssign = new DealerAssignVm();
    //            //GroupMemberAssign = new GroupAssignModel();
    //            ProductFamilySelected = new List<string>();
    //            //MemberSelected = new List<string>();          
    //        }

    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
    //        public string DealerId { get; set; }

    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerName", Required = Required.Default)]
    //        public string DealerName { get; set; }
    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerDescription", Required = Required.Default)]
    //        public string DealerDescription { get; set; }
    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "SAPID", Required = Required.Default)]
    //        public string SAPID { get; set; }


    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ProductFamilyAssign", Required = Required.Default)]
    //        public DealerAssignVm ProductFamilyAssign { get; set; }

    //        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "groupmemberassign", Required = Required.Default)]
    //        //public DealerAssignVm GroupMemberAssign { get; set; }

    //        [JsonProperty(PropertyName = "ProductFamilySelected", NullValueHandling = NullValueHandling.Ignore)]
    //        public List<string> ProductFamilySelected { get; set; }

    //        //[JsonProperty(PropertyName = "memberselected", NullValueHandling = NullValueHandling.Ignore)]
    //        //public List<string> MemberSelected { get; set; }
    //        public override string ToString()
    //        {
    //            return JsonConvert.SerializeObject(this);
    //        }
    //    }

    //    public class DealerAssignVm
    //    {
    //        public DealerAssignVm()
    //        {
    //            //AssignList = new Dictionary<string, string>();
    //            ProductFamilyDealer=new List<ProductFamilyCosmos>();
    //        }

    //        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "assignfor", Required = Required.Default)]
    //        public string AssignFor { get; set; }

    //        [JsonProperty(PropertyName = "assignlist", NullValueHandling = NullValueHandling.Ignore)]
    //        //public Dictionary<string, string> AssignList { get; set; }
    //        public List<ProductFamilyCosmos> ProductFamilyDealer { get; set; }


    //    }
    //}


    //public class DealerAssignedModel
    //{
    //    public DealerAssignedModel()
    //    {
    //        SelectedDealers = new List<string>();
    //    }

    //    //[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dealerId", Required = Required.Default)]
    //    //public string DealerId { get; set; }

    //    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "selectedDealers", Required = Required.Default)]
    //    public List<string> SelectedDealers { get; set; }

    //    //public override string ToString()
    //    //{
    //    //    return JsonConvert.SerializeObject(this);
    //    //}





}
