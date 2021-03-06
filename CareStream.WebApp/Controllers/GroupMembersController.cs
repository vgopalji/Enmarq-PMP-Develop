﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareStream.WebApp.Controllers
{
    public class GroupMembersController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IGroupMemberService _groupMemberService;

        public GroupMembersController(IGroupMemberService groupMemberService, ILoggerManager logger)
        {
            _groupMemberService = groupMemberService;
            _logger = logger;
        }

        // GET: GroupMembers
        public async Task<IActionResult> Index(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    TempData["GroupId"] = id;

                    var groupMembers = await _groupMemberService.GetGroupMembers(id);

                    if (groupMembers != null)
                    {
                        return View(groupMembers);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GroupMembersController-Index: Exception occurred .....");
                _logger.LogError(ex);
            }
            
            return View(new GroupMemberModel());
        }
        
        public async Task<ActionResult> AddMemberAsync(List<string> selectedMember)
        {
            var groupId = TempData["GroupId"];
            try
            {
                if (selectedMember != null)
                {
                    if (selectedMember.Any() && groupId != null)
                    {
                        var groupMemberAssignModel = new GroupMemberAssignModel();
                        groupMemberAssignModel.GroupId = groupId.ToString();
                        groupMemberAssignModel.SelectedMembers = selectedMember;

                        await _groupMemberService.AddGroupMembers(groupMemberAssignModel);
                    }
                }

                return RedirectToAction(nameof(Index),new { id= groupId.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError($"GroupMembersController-AddMemberAsync: Exception occurred ....");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index), new { id = groupId.ToString() });
            }
        }

        public async Task<ActionResult> GroupMemberDelete(List<string> selectedUser)
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
                            var val = new GroupMemberAssignModel();
                            val.GroupId = id;
                            val.SelectedMembers = selectedUser;

                            await _groupMemberService.RemoveGroupMembers(val);

                            return RedirectToAction(nameof(Index), new { id = id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GroupOwnersController-GroupOwnerDelete: Exception occurred ....");
                _logger.LogError(ex);
            }

            return RedirectToAction(nameof(Index), new { id = id });
        }
    }
}