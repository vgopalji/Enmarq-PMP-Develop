using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace CareStream.Models.RolesAndPermissions
{
    [Table("Permission")]
    public class Permission
    {
        [Key]
        public long PermissionId { get; set; }

        [ForeignKey("FK_Role_Permission")]
        public long RoleId { get; set; }

        [NotNull]
        public string UserId { get; set; }

        public bool Read { get; set; }

        public bool Write { get; set; }

        public bool ReadWrite { get; set; }

        public bool Delete { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }
}
