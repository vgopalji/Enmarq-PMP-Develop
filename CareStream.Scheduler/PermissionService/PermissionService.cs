using CareStream.Models.RolesAndPermissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CareStream.Scheduler.PermissionService
{
    public interface IPermissionService
    {
        List<PermissionModel> GetPermissionsByUser(string userId);

        bool SavePermissions(RolePermissionModel rolePermissionModel, string loginUserId);

        List<RoleModel> GetRoles();
    }

    public class PermissionService : IPermissionService
    {
        public List<PermissionModel> GetPermissionsByUser(string userId)
        {
            var dbContext = DbHelper.GetCareStreamContext();

            var permissions = dbContext.Permissions.Where(x => x.UserId == userId).ToList();

            var models = GetPermissionModels(permissions);

            return models;
        }

        public bool SavePermissions(RolePermissionModel rolePermissionModel, string loginUserId)
        {
            var dbContext = DbHelper.GetCareStreamContext();

            if (rolePermissionModel != null && rolePermissionModel.Permissions != null && rolePermissionModel.Permissions.Count > 0)
            {
                rolePermissionModel.Permissions.ForEach(x => { x.UserId = rolePermissionModel.UserId; });
                var permissions = GetPermissions(rolePermissionModel.Permissions, loginUserId);
                var addPermissions = permissions.Where(x => x.PermissionId == 0).ToList();
                dbContext.AddRange(addPermissions);

                var existingPermissionModels = rolePermissionModel.Permissions.Where(x => x.PermissionId > 0).ToDictionary(x => x.PermissionId, y => y);
                if (existingPermissionModels.Count > 0)
                {
                    var existingPermissionIds = existingPermissionModels.Select(x => x.Key);
                    var existingPermissions = dbContext.Permissions.Where(x => existingPermissionIds.Contains(x.PermissionId)).ToList();
                    if (existingPermissions.Any())
                    {
                        existingPermissions.ForEach(x =>
                        {
                            var model = existingPermissionModels[x.PermissionId];
                            x.ModifiedBy = loginUserId;
                            x.ModifiedDate = DateTime.Now;
                            x.Read = model.Read;
                            x.ReadWrite = model.ReadWrite;
                            x.Delete = model.Delete;
                            x.Write = model.Write;
                            x.UserId = model.UserId;
                            x.RoleId = model.RoleId;
                        });
                    }
                }

                dbContext.SaveChanges();
            }

            return true;
        }

        public List<RoleModel> GetRoles()
        {
            var dbContext = DbHelper.GetCareStreamContext();

            var roles = dbContext.Roles.ToList();

            var roleModels = new List<RoleModel>();

            if (roles.Count > 0)
            {
                roles.ForEach(x =>
                {
                    roleModels.Add(new RoleModel
                    {
                        RoleId = x.RoleId,
                        RoleType = x.RoleType,
                        RoleSection = x.RoleSection
                    });
                });
            }

            return roleModels;
        }

        private List<Permission> GetPermissions(List<PermissionModel> permissionModels, string userId)
        {
            var permissions = new List<Permission>();

            permissionModels.ForEach(x =>
            {
                permissions.Add(new Permission
                {
                    PermissionId = x.PermissionId,
                    RoleId = x.RoleId,
                    UserId = x.UserId,
                    Read = x.Read,
                    Write = x.Write,
                    ReadWrite = x.ReadWrite,
                    Delete = x.Delete,
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = userId,
                    ModifiedDate = DateTime.Now
                });
            });

            return permissions;
        }

        private List<PermissionModel> GetPermissionModels(List<Permission> permissions)
        {
            var permissionModels = new List<PermissionModel>();

            permissions.ForEach(x =>
            {
                permissionModels.Add(new PermissionModel
                {
                    PermissionId = x.PermissionId,
                    RoleId = x.RoleId,
                    UserId = x.UserId,
                    Read = x.Read,
                    Write = x.Write,
                    ReadWrite = x.ReadWrite,
                    Delete = x.Delete
                });
            });

            return permissionModels;
        }
    }
}
