using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models.BulkFile
{
   public class UserFile
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
