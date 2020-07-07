using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models.BulkFile
{
   public class Users
    {
        public long Id { get; set; }
        public long FileId { get; set; }
        public string UserPrincipalName { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
