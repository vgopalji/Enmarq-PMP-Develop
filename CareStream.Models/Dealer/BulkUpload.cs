using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CareStream.Models.Dealer
{
    public class BulkUpload
    {
        public string DealerName { get; set; }
        public string DealerDescription { get; set; }
        public string SAPID { get; set; }
        public string ProductFamilyName { get; set; }
    }

    public class FileDetails
    {
        [Key]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "FileDetailsId", Required = Required.Default)]
        public string FileDetailsId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "CreatedDate", Required = Required.Default)]
        public DateTime CreatedDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "FileName", Required = Required.Default)]
        public string FileName { get; set; }
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "FileSize", Required = Required.Default)]
        //public string FileSize { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "UploadBy", Required = Required.Default)]
        public string UploadBy { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Status", Required = Required.Default)]
        public string Status { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Success", Required = Required.Default)]
        public string Success { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "Failure", Required = Required.Default)]
        public string Failure { get; set; }
    }
}
