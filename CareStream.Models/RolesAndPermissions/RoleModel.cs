using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models.RolesAndPermissions
{
    public class RoleModel
    {
        public long RoleId { get; set; }

        public string RoleType { get; set; }

        public string RoleSection { get; set; }
    }
}
