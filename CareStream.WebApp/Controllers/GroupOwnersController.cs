﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Mvc;


namespace CareStream.WebApp.Controllers
{
    public class GroupOwnersController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IGroupOwnerService _groupOwnerService;

        public GroupOwnersController(IGroupOwnerService groupOwnerService, ILoggerManager logger)
        {
            _groupOwnerService = groupOwnerService;
            _logger = logger;
        }
        // GET: GroupOwners
        public async Task<IActionResult> Index(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    TempData["GroupId"] = id;

                    var groupOwners = await _groupOwnerService.GetGroupOwners(id);

                    if (groupOwners != null)
                    {
                        return View(groupOwners);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GroupOwnersController-Index: Exception occurred .....");
                _logger.LogError(ex);
            }

            return View(new GroupOwnerModel());
        }

        public async Task<ActionResult> AddOwnerAsync(List<string> selectedOwner)
        {
            var groupId = TempData["GroupId"];
            try
            {
                if (selectedOwner != null)
                {
                    if (selectedOwner.Any() && groupId != null)
                    {
                        var groupOwnerAssignModel = new GroupOwnerAssignModel();
                        groupOwnerAssignModel.GroupId = groupId.ToString();
                        groupOwnerAssignModel.SelectedOwners = selectedOwner;

                        await _groupOwnerService.AddGroupOwners(groupOwnerAssignModel);
                    }
                }

                return RedirectToAction(nameof(Index), new { id = groupId.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GroupOwnersController-AddOwnerAsync: Exception occurred ....");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index), new { id = groupId.ToString() });
            }
        }

        public async Task<ActionResult> GroupOwnerDelete(List<string> selectedUser)
        {
            var id = string.Empty;
            try
            {
                var groupId = TempData["GroupId"];

                if (groupId != null)
                {
                    id = groupId.ToString();
                    if (selectedUser != null)
                    {
                        if (selectedUser.Any())
                        {
                            GroupOwnerAssignModel val = new GroupOwnerAssignModel
                            {
                                GroupId = id,
                                SelectedOwners = selectedUser
                            };

                            await _groupOwnerService.RemoveGroupOwners(val);

                            return RedirectToAction(nameof(Index), new { id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GroupOwnersController-GroupOwnerDelete: Exception occurred ....");
                _logger.LogError(ex);
            }

            return RedirectToAction(nameof(Index), new { id });
        }
    }
}