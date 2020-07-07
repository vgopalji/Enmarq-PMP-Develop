using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Graph;
using CareStream.WebApp.Extensions;

namespace CareStream.WebApp.Controllers
{
    public class UsersController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IUserService _userService;
        private readonly IUserAttributeService _userAttributeService;

        public UsersController(IUserService userService, ILoggerManager logger, IUserAttributeService userAttributeService)
        {
            _logger = logger;
            _userService = userService;
            _userAttributeService = userAttributeService;
        }

        public async Task<IActionResult> UsersList()
        {
            try
            {
                var usersModel = await GetUsers() as OkObjectResult;

                if (usersModel.Value != null)
                {
                    return View(usersModel.Value);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                _logger.LogError("UsersController-Index: Exception occurred...");
                _logger.LogError(ex);
            }

            return View(new UsersModel());
        }

        public async Task<IActionResult> DeletedUsers()
        {
            var usersModel = await GetDeletedUsers() as OkObjectResult;

            if (usersModel.Value != null)
            {
                return View(usersModel.Value);
            }
            return View("DeletedUsers", new UsersModel());
        }

        [BindProperty]
        public UserModel userModel { get; set; }

        public async Task<IActionResult> Create()
        {
            try
            {
                await BindViewDataForUser();
                var userModel = new UserViewModel();

                if (userModel != null)
                {
                    return View(userModel);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController-Create: Exception occurred...");
                _logger.LogError(ex);
            }

            return View("Create", new UserModel());
        }

        public IActionResult Invite()
        {
            TempData["activeTab"] = "invite";
            return RedirectToAction("Create");
        }

        private async Task BindViewDataForUser()
        {
            try
            {
                #region Default Selected List
                var gItems = new SelectList(new List<SelectListItem>());
                var roleItems = new SelectList(new List<SelectListItem>());
                var userLocationItems = new SelectList(new List<SelectListItem>());
                var userTypeItems = new SelectList(new List<SelectListItem>());
                var languageItems = new SelectList(new List<SelectListItem>());
                var userBusDepartmentItems = new SelectList(new List<SelectListItem>());
                var userAttributes = new SelectList(new List<SelectListItem>());
                #endregion

                var taskUserModel = await GetUserDropDown();

                if (taskUserModel != null)
                {
                    var userDropDownModel = taskUserModel;

                    //userModel.AutoGeneratePassword = true;
                    //userModel.Password = userDropDownModel.AutoPassword;

                    #region Group 

                    if (userDropDownModel.Groups != null)
                    {
                        var itemList = new List<SelectListItem>();
                        foreach (var group in userDropDownModel.Groups)
                        {
                            try
                            {
                                var groupListItem = new SelectListItem
                                {
                                    Text = group.Key,
                                    Value = group.Key
                                };

                                itemList.Add(groupListItem);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error while assign group key {0}", group.Key);
                                Console.WriteLine(ex);
                            }
                        }

                        gItems = new SelectList(itemList, "Value", "Text");
                    }

                    #endregion

                    #region UserAttributes 

                    var attributes = new List<UserType>();
                    var schemaAttributes = await _userAttributeService.GetUserAttribute();
                    schemaAttributes.Properties.ForEach(ext => {
                        attributes.Add(new UserType
                        {
                            Key = ext.Name,
                            Value = ext.DataType
                        });

                    });
                    #endregion

                    ViewData["Group"] = gItems;
                    ViewData["Roles"] = roleItems;
                    ViewData["UserLocation"] = userLocationItems;
                    ViewData["UserType"] = userTypeItems;
                    ViewData["Language"] = languageItems;
                    ViewData["UserBusDep"] = userBusDepartmentItems;
                    ViewData["UserAttributes"] = attributes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Expection occured while getting user drop down details");
                Console.WriteLine(ex);
            }

        }

        public async Task<IActionResult> GetFilteredUsers(string id)
        {
            var usersModel = await _userService.GetFilteredUsers(id);

            return RedirectToAction("UsersList", usersModel);
        }

        public async Task<IActionResult> GetUsers()
        {
            var usersModel = await _userService.GetUser();

            return Ok(usersModel);
        }

        public async Task<IActionResult> GetDeletedUsers()
        {
            var usersModel = await _userService.GetDeletedUser();
            return Ok(usersModel);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userViewModel = new UserViewModel();

            try
            {
                await BindViewDataForUser();

                if ("me".Equals(id))
                    id = this.User.GetEmail();

                var userModel = await _userService.GetUser(id);
               
                #region UserAttributes 

                var attributes = new List<UserType>();
                var schemaAttributes = await _userAttributeService.GetUserAttribute();
                schemaAttributes.Properties.ForEach(ext => {
                    attributes.Add(new UserType
                    {
                        Key = ext.Name,
                        Value = ext.DataType
                    });

                });
                ViewData["UserAttributes"] = attributes;
                #endregion
                userViewModel.UserModel = userModel;
                return View(userViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController-Create: Exception occurred...");
                _logger.LogError(ex);
            }

            return View("Edit", userViewModel);
        }

        public IActionResult ResetPassword()
        {
            return RedirectToAction("ResetPassword", "Account", new { area = "AzureADB2C" });
        }

        #region User 

        public async Task<UserDropDownModel> GetUserDropDown()
        {
            UserDropDownModel userDropDownModel = new UserDropDownModel();

            try
            {
                userDropDownModel = await _userService.GetUserDropDownAsync();

                if (userDropDownModel == null)
                    userDropDownModel = new UserDropDownModel();
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController-GetUserDropDown: Exception occurred...");
                _logger.LogError(ex);
            }

            return userDropDownModel;
        }

        #endregion

        public async Task<IActionResult> Post([FromForm] UserViewModel UserView)
        {
            try
            {
                if (UserView.UserModel.PasswordOptionType == CareStream.Models.User.PasswordOptionType.ManualCreatedPassword)
                {
                    if (string.IsNullOrEmpty(UserView.UserModel.Password) || !(new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,20}$")).IsMatch(UserView.UserModel.Password))
                        ModelState.AddModelError("UserModel.Password", "Please enter valid Password");
                    else
                        ModelState.Remove("UserModel.Password");
                }

                if (!ModelState.IsValid)
                {
                    await BindViewDataForUser();
                    return View("Create", UserView);
                }

                await _userService.Create(UserView.UserModel);

                ShowSuccessMessage("Succssfully Created the User.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("User creation failed: " + ex.Message);
                _logger.LogError("UsersController-Post: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction("UsersList");
        }

        public async Task<IActionResult> PostEdit([FromForm] UserViewModel UserView)
        {
            try
            {
                if (UserView.UserModel.PasswordOptionType == CareStream.Models.User.PasswordOptionType.ManualCreatedPassword)
                {
                    if (string.IsNullOrEmpty(UserView.UserModel.Password) || !(new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,20}$")).IsMatch(UserView.UserModel.Password))
                        ModelState.AddModelError("UserModel.Password", "Please enter valid Password");
                    else
                        ModelState.Remove("UserModel.Password");
                }

                if (!ModelState.IsValid)
                {
                    await BindViewDataForUser();
                    return View("Edit", UserView);
                }

                await _userService.Update(UserView.UserModel);

                ShowSuccessMessage("Succssfully Updated the User.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("User updation failed: " + ex.Message);
                _logger.LogError("UsersController-Post: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction("UsersList");
        }

        public async Task<IActionResult> CreateInvite([FromForm] UserViewModel user)
        {
            try
            {
                await _userService.SendInvite(user.InviteUser);
                ShowSuccessMessage("Succssfully Invited the User.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("User invite failed: " + ex.Message);
                _logger.LogError("UsersController-CreateInvite: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction("UsersList");
        }

        public async Task<ActionResult> UserDelete(List<string> selectedItems)
        {
            try
            {
                if (selectedItems != null)
                {
                    if (selectedItems.Any())
                    {
                        await _userService.RemoveUser(selectedItems);

                        ShowSuccessMessage("Succssfully Deleted the User.");
                        return RedirectToAction(nameof(UsersList));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("User deletion failed: " + ex.Message);
                _logger.LogError("UsersController-UserDelete: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction(nameof(UsersList));
        }
    }
}