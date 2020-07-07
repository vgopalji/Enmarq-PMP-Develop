using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareStream.Models.RolesAndPermissions
{
    [Table("Role")]
    public class Role
    {
        [Key]
        public long RoleId { get; set; }

        [StringLength(100)]
        public string RoleType { get; set; }

        [StringLength(100)]
        public string RoleSection { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }
    }

    public enum RoleSection
    {
        Users = 1,
        Groups,
        UserAttributes,
        BulkOperations
    }
}
