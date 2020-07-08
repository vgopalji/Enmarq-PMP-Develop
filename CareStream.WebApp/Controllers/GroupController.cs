using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareStream.WebApp.Controllers
{
    public class GroupController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService, ILoggerManager logger)
        {
            _logger = logger;
            _groupService = groupService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var groups = await _groupService.GetDetailGroupList();

                if (groups != null)
                {
                    return View(groups);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("Index", new List<GroupModel>());
                //comment
            }

            return View("Index", new List<GroupModel>());
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var group = await _groupService.GetGroup(id);

                    if (group != null)
                    {
                        return View(group);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupController-Details: Exception occurred...");
                _logger.LogError(ex);
                return View(new GroupModel());
            }

            return View(new GroupModel());
        }

        public async Task<IActionResult> Create()
        {
            var groupModel = new GroupModel();
            try
            {
                TempData.Clear();
                var groupOwnerMemberResult = await _groupService.BuildGroupOwnerMember();

                if (groupOwnerMemberResult != null)
                {
                    if (groupOwnerMemberResult.Any())
                    {
                        foreach (var groupOwnerMember in groupOwnerMemberResult)
                        {
                            try
                            {
                                switch (groupOwnerMember.AssignFor)
                                {
                                    case CareStreamConst.Owner:
                                        groupModel.GroupOwnerAssign = groupOwnerMember;
                                        break;
                                    case CareStreamConst.Member:
                                        groupModel.GroupMemberAssign = groupOwnerMember;
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"GroupController-Create: Exception occurred while adding the Group Assigment for {groupOwnerMember.AssignFor}");
                                _logger.LogError(ex);
                            }
                        }
                    }
                }

                BindViewDataForGroup(groupModel);

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupController-Create: Exception occurred...");
                _logger.LogError(ex);
                return View(groupModel);
            }

            return View(groupModel);
        }

        #region Binding View Data for Create Group

        private void BindViewDataForGroup(GroupModel groupModel)
        {
            try
            {
                if (groupModel.GroupTypes != null)
                {
                    var groupItems = new SelectList(new List<SelectListItem>());

                    var itemList = new List<SelectListItem>();
                    foreach (var groupType in groupModel.GroupTypes)
                    {
                        try
                        {
                            var groupTypeListItem = new SelectListItem
                            {
                                Text = groupType,
                                Value = groupType
                            };

                            itemList.Add(groupTypeListItem);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error while assign user group type for key {0}", groupType);
                            Console.WriteLine(ex);
                        }
                    }

                    groupItems = new SelectList(itemList, "Value", "Text");
                    ViewData["GroupType"] = groupItems;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expection occured while getting group type details");
                Console.WriteLine(ex);
            }
        }

        #endregion

        public void AddSelectedOwnerOrMember([FromBody] GroupMemberAssignModel groupMemberAssign)
        {
            if (groupMemberAssign != null)
            {
                if (groupMemberAssign.SelectedMembers != null && !string.IsNullOrEmpty(groupMemberAssign.GroupId))
                {

                    switch (groupMemberAssign.GroupId)
                    {
                        case CareStreamConst.Member:
                            TempData[CareStreamConst.GroupMember] = groupMemberAssign.SelectedMembers;
                            break;
                        case CareStreamConst.Owner:
                            TempData[CareStreamConst.GroupOwner] = groupMemberAssign.SelectedMembers;
                            break;
                    }

                }
            }

        }

        public async Task<IActionResult> Post([FromForm] GroupModel groupModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    BindViewDataForGroup(groupModel);
                    return View("Create", groupModel);
                }

                if (groupModel != null)
                {
                    if (string.IsNullOrEmpty(groupModel.DisplayName) || string.IsNullOrEmpty(groupModel.GroupType))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                switch (groupModel.GroupType)
                {
                    case CareStreamConst.O365:
                        groupModel.GroupTypes = new List<string>
                            {
                                CareStreamConst.Unified
                            };
                        groupModel.MailEnabled = true;
                        groupModel.SecurityEnabled = false;
                        break;
                    case CareStreamConst.Security:
                        groupModel.GroupTypes = new List<string>();
                        groupModel.MailEnabled = false;
                        groupModel.SecurityEnabled = true;
                        break;
                }
                groupModel.Visibility = CareStreamConst.Public;

                var displayNames = Regex.Split(groupModel.DisplayName, @"\W+");

                if (displayNames.Count() > 0)
                    groupModel.MailNickname = string.Join('_', displayNames);


                if (TempData[CareStreamConst.GroupMember] != null)
                {
                    if (TempData[CareStreamConst.GroupMember] is string[])
                    {
                        var selectMember = TempData[CareStreamConst.GroupMember] as string[];
                        if (selectMember != null)
                        {
                            groupModel.MemberSelected = selectMember.ToList();
                        }
                    }
                }

                if (TempData[CareStreamConst.GroupOwner] != null)
                {
                    if (TempData[CareStreamConst.GroupOwner] is string[])
                    {
                        var selectOwner = TempData[CareStreamConst.GroupOwner] as string[];
                        if (selectOwner != null)
                        {
                            groupModel.OwnerSelected = selectOwner.ToList();
                        }
                    }
                }

                var newGroup = await _groupService.CreateGroup(groupModel);

                ShowSuccessMessage("Succssfully Created the Group.");

                if (newGroup != null)
                {
                    return RedirectToAction(nameof(Details), new { id = newGroup.ObjectId });
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Group creation failed: " + ex.Message);
                TempData.Clear();
                _logger.LogError("GroupController-Post: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> GroupDelete(List<string> selectedItems)
        {
            try
            {
                if (selectedItems != null && selectedItems.Count != 0)
                {

                    if (selectedItems.Any())
                    {
                        await _groupService.RemoveGroup(selectedItems);

                        return Ok(GetSuccessMessage("Succssfully deleted the Group."));
                    }
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage("Group deletion failed: " + ex.Message);
                _logger.LogError("GroupController-GroupDelete: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return Ok();
        }
    }
}