using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models.RolesAndPermissions;
using CareStream.Scheduler.PermissionService;
using CareStream.Utility;
using CareStream.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareStream.WebApp.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IPermissionService _permissionService;
        private readonly IUserService _userService;

        public PermissionController(ILoggerManager logger, IPermissionService permissionService, IUserService userService)
        {
            _logger = logger;
            _permissionService = permissionService;
            _userService = userService;
        }

        public async Task<IActionResult> Update()
        {
            try
            {
                await BuildViewDataForPermissions();
                var rolePermissionModel = new RolePermissionModel();
                rolePermissionModel.PermissionAction = "Save";

                return View(rolePermissionModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("PermissionController-Update: Exception occurred...");
                _logger.LogError(ex);

                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(RolePermissionModel rolePermissionModel)
        {
            try
            {
                if (rolePermissionModel.UserId != null && string.Equals(rolePermissionModel.PermissionAction, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    var permissions = GetPermissions(rolePermissionModel.UserId);
                    rolePermissionModel.Permissions = permissions;
                    rolePermissionModel.PermissionAction = "Save";
                }
                else if (rolePermissionModel.Permissions != null && rolePermissionModel.Permissions.Count > 0 && string.Equals(rolePermissionModel.PermissionAction, "Save", StringComparison.OrdinalIgnoreCase))
                {
                    var userId = GetUserId();
                    _permissionService.SavePermissions(rolePermissionModel, userId);
                    ShowSuccessMessage("User Permissions updated successfuly.");
                }

                await BuildViewDataForPermissions();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PermissionController:Update:Exception occured while updating the user permissions...");
                _logger.LogError(ex);
                ShowErrorMessage(ex.Message);

                throw ex;
            }

            return View(rolePermissionModel);
        }

        private List<PermissionModel> GetPermissions(string userId)
        {
            try
            {
                var permissions = _permissionService.GetPermissionsByUser(userId);

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PermissionController:GetPermissions:Exception occured while getting the user permissions...");
                _logger.LogError(ex);

                throw ex;
            }
        }

        private async Task BuildViewDataForPermissions()
        {
            var roles = _permissionService.GetRoles();
            var usersModel = await _userService.GetUser();

            var userItems = new SelectList(new List<SelectListItem>());

            if (usersModel != null && usersModel.Users != null)
            {
                var itemList = new List<SelectListItem>();
                var initialSelect = new SelectListItem
                {
                    Text = string.Empty,
                    Value = string.Empty
                };

                itemList.Add(initialSelect);
                foreach (var user in usersModel.Users)
                {
                    try
                    {
                        var userListItem = new SelectListItem
                        {
                            Text = user.GivenName,
                            Value = user.Id
                        };

                        itemList.Add(userListItem);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while assign user for key {0}", user.GivenName);
                        Console.WriteLine(ex);
                    }
                }

                userItems = new SelectList(itemList, "Value", "Text");
            }

            TempData["Roles"] = roles;

            TempData["Users"] = userItems;
        }
    }
}
