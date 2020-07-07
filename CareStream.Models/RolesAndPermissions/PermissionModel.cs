using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CareStream.Models.RolesAndPermissions
{
    public class PermissionModel
    {
        public long PermissionId { get; set; }

        [Required]
        public long RoleId { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool Read { get; set; }

        public bool Write { get; set; }

        public bool ReadWrite { get; set; }

        public bool Delete { get; set; }
    }
}
