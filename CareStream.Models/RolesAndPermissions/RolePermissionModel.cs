using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models.RolesAndPermissions
{
    public class RolePermissionModel
    {
        public RolePermissionModel()
        {
            Permissions = new List<PermissionModel>();
        }

        public string UserId { get; set; }

        public string PermissionAction { get; set; }

        public List<PermissionModel> Permissions { get; set; }
    }
}
