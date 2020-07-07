using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CareStream.Utility
{
    public interface IUserService
    {
        Task<UsersModel> GetUser();

        Task<UsersModel> GetDeletedUser();

        Task<UserDropDownModel> GetUserDropDownAsync();

        Task SendInvite(InviteUser user);

        Task Create(UserModel user);

        Task RemoveUser(List<string> userIdsToDelete);

        Task<UsersModel> GetFilteredUsers(string filter);

        Task<UserModel> GetUser(string id);

        Task Update(UserModel user);
    }

    public class UserService : IUserService
    {
        private readonly ILoggerManager _logger;
        public UserService(ILoggerManager logger)
        {
            _logger = logger;
        }

        public UserService()
        {
            _logger = new LoggerManager();
        }

        #region MVC 

        public async Task RemoveUser(List<string> userIdsToDelete)
        {
            try
            {
                if (userIdsToDelete == null)
                {
                    _logger.LogError("UserService-RemoveUser: Input value cannot be empty");
                    return;
                }

                GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();
                if (client == null)
                {
                    _logger.LogError("UserService-RemoveUser: Unable to create object for graph client");
                    return;
                }

                foreach (var id in userIdsToDelete)
                {
                    try
                    {
                        _logger.LogInfo($"UserService-RemoveUser: [Started] removing user for id [{id}] on Azure AD B2C");

                        await client.Users[id].Request().DeleteAsync();

                        _logger.LogInfo($"UserService-RemoveUser: [Completed] removing user [{id}] on Azure AD B2C");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"UserService-RemoveUser: Exception occured while removing user for id [{id}]");
                        _logger.LogError(ex);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-RemoveUser: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        #endregion

        #region API Helper

        public User BuildUserForCreation(UserModel userModel, string tenantId)
        {
            User newUser = null;
            try
            {
                if (userModel == null)
                {
                    _logger.LogError("UserService-BuildUserForCreation: Input value cannot be null");
                    return newUser;
                }

                _logger.LogInfo($"UserService-BuildUserForCreation: Building User Object for {userModel.SignInName}");

                if (string.IsNullOrEmpty(userModel.Password))
                {

                    _logger.LogWarn($"UserService-BuildUserForCreation: User {userModel.SignInName} has not set the password. System is assigning automatic password");
                    userModel.Password = GetRandomPassword();
                }

                var nickName = getMailNickName(userModel.GivenName, userModel.Surname);


                //How to get the delegated administration
                newUser = new User();
                newUser.GivenName = userModel.GivenName;
                newUser.Surname = userModel.Surname;
                newUser.DisplayName = userModel.DisplayName;
                newUser.AccountEnabled = userModel.AccountEnabled;

                var mailNickName = string.IsNullOrEmpty(nickName) ? newUser.DisplayName : nickName;

                newUser.UserPrincipalName = userModel.UserPrincipalName;

                newUser.MailNickname = mailNickName;
                newUser.UsageLocation = userModel.UsageLocation;
                newUser.JobTitle = userModel.JobTitle;
                newUser.Department = userModel.Department;
                if (!string.IsNullOrEmpty(userModel.StreetAddress))
                {
                    newUser.StreetAddress = userModel.StreetAddress;
                }
                else
                {
                    newUser.StreetAddress = $"{userModel.Address} {userModel.Address2} {userModel.Address3}";
                }

                newUser.State = userModel.State;
                newUser.Country = userModel.Country;
                newUser.PostalCode = userModel.PostalCode;
                newUser.City = userModel.City;

                newUser.MobilePhone = userModel.MobilePhone;
                //newUser.MySite = userModel.WebSite;
                newUser.Mail = userModel.SignInName;
                newUser.UserPrincipalName = $"{newUser.MailNickname}@{GraphClientUtility.Domain}";

                if (!string.IsNullOrEmpty(userModel.BusinessPhone))
                    newUser.BusinessPhones = new List<string> { userModel.BusinessPhone };

                newUser.Identities = new List<ObjectIdentity>
                {
                        new ObjectIdentity()
                        {
                            SignInType =  CareStreamConst.UserSignInType_UserPrincipalName,
                            Issuer = tenantId,
                            IssuerAssignedId = $"{mailNickName}@{tenantId}"
                        },
                        new ObjectIdentity()
                        {
                            SignInType =  CareStreamConst.UserSignInType_EmailAddress,
                            Issuer = tenantId,
                            IssuerAssignedId = userModel.SignInName
                        }
                };

                newUser.PasswordProfile = new PasswordProfile()
                {
                    Password = userModel.Password,
                    ForceChangePasswordNextSignIn = userModel.ForceChangePasswordNextSignIn
                };

                _logger.LogInfo($"UserService-BuildUserForCreation: Completed building User Object for {userModel.SignInName}");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-BuildUserForCreation: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return newUser;
        }

        public User BuildUserForUpdate(UserModel userModel, string b2cExtensionAppClientId)
        {
            User updateUser = null;
            try
            {
               
                if (userModel == null)
                {
                    _logger.LogError("UserService-BuildUserForUpdate: Input value cannot be null");
                    return updateUser;
                }

                _logger.LogInfo($"UserService-BuildUserForUpdate: Building User Object for {userModel.SignInName}");

                #region Update User
                var nickName = getMailNickName(userModel.GivenName, userModel.Surname);

                updateUser = new User();
                updateUser.Id = userModel.Id;
                updateUser.Identities = new List<ObjectIdentity>
                {
                        new ObjectIdentity()
                        {
                            SignInType =  CareStreamConst.UserSignInType_UserPrincipalName,
                            Issuer = GraphClientUtility.Domain,
                            IssuerAssignedId = userModel.UserPrincipalName
                        },
                        new ObjectIdentity()
                        {
                            SignInType =  CareStreamConst.UserSignInType_EmailAddress,
                            Issuer = GraphClientUtility.Domain,
                            IssuerAssignedId = userModel.SignInName
                        }
                };

                if (!string.IsNullOrEmpty(userModel.GivenName))
                {
                    updateUser.GivenName = userModel.GivenName;
                }

                if (!string.IsNullOrEmpty(userModel.Surname))
                {
                    updateUser.Surname = userModel.Surname;
                }

                if (!string.IsNullOrEmpty(userModel.DisplayName))
                {
                    updateUser.DisplayName = userModel.DisplayName;
                }

                if (userModel.AccountEnabled != null)
                {
                    updateUser.AccountEnabled = userModel.AccountEnabled;
                }

                if (!string.IsNullOrEmpty(nickName))
                {
                    updateUser.MailNickname = nickName;
                }

                if (!string.IsNullOrEmpty(userModel.UsageLocation))
                {
                    updateUser.UsageLocation = userModel.UsageLocation;
                }

                if (!string.IsNullOrEmpty(userModel.SignInName))
                {
                    updateUser.UserPrincipalName = userModel.SignInName;
                }

                if (!string.IsNullOrEmpty(userModel.JobTitle))
                {
                    updateUser.JobTitle = userModel.JobTitle;
                }

                if (!string.IsNullOrEmpty(userModel.Department))
                {
                    updateUser.Department = userModel.Department;
                }

                if (!string.IsNullOrEmpty(userModel.StreetAddress))
                {
                    updateUser.StreetAddress = userModel.StreetAddress;
                }
                else
                {
                    updateUser.StreetAddress = $"{userModel.Address} {userModel.Address2} {userModel.Address3}";
                }

                if (!string.IsNullOrEmpty(userModel.State))
                {
                    updateUser.State = userModel.State;
                }

                if (!string.IsNullOrEmpty(userModel.Country))
                {
                    updateUser.Country = userModel.Country;
                }

                if (!string.IsNullOrEmpty(userModel.PostalCode))
                {
                    updateUser.PostalCode = userModel.PostalCode;
                }

                if (!string.IsNullOrEmpty(userModel.City))
                {
                    updateUser.PostalCode = userModel.PostalCode;
                }


                if (!string.IsNullOrEmpty(userModel.MobilePhone))
                {
                    updateUser.MobilePhone = userModel.MobilePhone;
                }

                if (!string.IsNullOrEmpty(userModel.WebSite))
                {
                    updateUser.MySite = userModel.WebSite;
                }

                if (!string.IsNullOrEmpty(userModel.BusinessPhone))
                {
                    updateUser.BusinessPhones = new List<string> { userModel.BusinessPhone };
                }

                if (userModel.OtherMails != null)
                {
                    if (userModel.OtherMails.Any())
                    {
                        updateUser.OtherMails = userModel.OtherMails;
                    }

                }

                if (!string.IsNullOrEmpty(userModel.Password))
                {
                    updateUser.PasswordProfile = new PasswordProfile()
                    {
                        Password = userModel.Password,
                        ForceChangePasswordNextSignIn = userModel.ForceChangePasswordNextSignIn
                    };
                }

                #endregion

                _logger.LogInfo($"UserService-BuildUserForCreation: Completed building User Object for {userModel.SignInName}");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-BuildUserForCreation: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return updateUser;
        }

        public bool IsUserModelValid(UserModel userModel)
        {
            if (userModel == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(userModel.SignInName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(userModel.DisplayName))
            {
                return false;
            }

            return true;
        }

        public void AssignGroupsToUser(GraphServiceClient client, User user, List<string> groups)
        {
            try
            {
                if (groups == null)
                {
                    _logger.LogWarn("UserService-AssignGroupsToUser: User group is null");
                    return;
                }

                if (!groups.Any())
                {
                    _logger.LogInfo("UserService-AssignGroupsToUser: No user group");
                    return;
                }

                _logger.LogInfo("UserService-AssignGroupsToUser: Starting to assign user to groups");

                var groupService = new GroupService(_logger, null, null);
                var allGroups = groupService.GetGroups();

                if (allGroups != null)
                {
                    _logger.LogInfo($"UserService-AssignGroupsToUser: Got {allGroups.Count}  groups from Azure AD B2C");

                    if (allGroups.Any())
                    {
                        UserGroup userGroup = new UserGroup
                        {
                            AllGroups = allGroups,
                            User = user,
                            Groups = groups
                        };

                        var userGroupService = new UserGroupService(_logger);
                        var assignResult = userGroupService.AssignUserToGroup(userGroup);

                        if (assignResult != null)
                        {
                            var groupAssignedResults = assignResult.Result.ToList<GroupAssigned>();

                            if (groupAssignedResults.Any(x => x.IsGroupAssigned == false))
                            {
                                _logger.LogWarn($"UserService-AssignGroupsToUser: Failed to assign one or more group for User Id: {user.Id}, see the detail above");
                            }
                            else
                            {
                                _logger.LogInfo($"UserService-AssignGroupsToUser: Assigned group(s) to user [with id {user.Id} on Azure AD B2C");
                            }

                        }
                    }
                    else
                    {
                        _logger.LogInfo("UserService-AssignGroupsToUser: No group found in Azure AD B2C");
                    }

                }
                _logger.LogInfo("UserService-AssignGroupsToUser: Completed to assign user to groups");
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-AssignGroupsToUser: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task<UserDropDownModel> GetUserDropDownAsync()
        {
            UserDropDownModel retVal = null;
            try
            {
                retVal = new UserDropDownModel();

                var client = GraphClientUtility.GetGraphServiceClient();

                var loadUserDropDownTasks = new Task[]
                {
                    Task.Run(() => {
                                         PasswordService.Logger = _logger;
                                         retVal.AutoPassword = PasswordService.GenerateNewPassword(GetRandomNumber(),
                                                                    GetRandomNumber(), GetRandomNumber(), GetRandomNumber());
                                     }),
                    Task.Run(() => {
                                        GroupService groupService = new GroupService(_logger, null, null);
                                        retVal.Groups = groupService.GetGroups();
                                     }),
                    Task.Run(() => retVal.UserRoles = GetUserRoles()),
                    Task.Run(() =>
                                {
                                    CountryService countryService = new CountryService(_logger);
                                    var countryResult = countryService.GetCountries();

                                    if(countryResult != null)
                                    {
                                        retVal.UserLocation = countryResult.Result.CountryModel;
                                    }
                                }),
                     Task.Run(() => retVal.UserTypes = GetUserTypes()),
                     Task.Run(() => retVal.UserLanguages = GetUserLanguage()),
                     Task.Run(() =>  retVal.UserBusinessDepartments = GetUserBusinessDepartments())
                };

                await Task.WhenAll(loadUserDropDownTasks);
            }
            catch (Exception ex)
            {
                retVal = null;
                _logger.LogError("UserService-GetUserDropDownAsync: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }

        public string GetRandomPassword()
        {
            var retVal = string.Empty;
            try
            {
                PasswordService.Logger = _logger;
                retVal = PasswordService.GenerateNewPassword(GetRandomNumber(), GetRandomNumber(), GetRandomNumber(), GetRandomNumber());
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-GetRandomPassword: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }

        public UserDropDownModel GetUserDropDown(GraphServiceClient client)
        {
            UserDropDownModel retVal = null;
            try
            {
                if (client == null)
                {
                    _logger.LogWarn("UserService-GetUserDropDown: Graph client cannot be null");
                    return retVal;
                }

                _logger.LogInfo("UserService-GetUserDropDown: Starting to get user drop down");

                retVal = new UserDropDownModel();

                // Password
                retVal.AutoPassword = GetRandomPassword();
                _logger.LogInfo("UserService-GetUserDropDown: Generated auto password for user drop down");

                //Groups 
                GroupService groupService = new GroupService(_logger, null, null);
                retVal.Groups = groupService.GetGroups();

                //Roles
                retVal.UserRoles = GetUserRoles();

                //User Location
                CountryService countryService = new CountryService(_logger);
                var countryResult = countryService.GetCountries();

                if (countryResult != null)
                {
                    retVal.UserLocation = countryResult.Result.CountryModel;
                }

                // User Type
                retVal.UserTypes = GetUserTypes();

                //User Language
                retVal.UserLanguages = GetUserLanguage();

                //User Business Department
                retVal.UserBusinessDepartments = GetUserBusinessDepartments();

                _logger.LogInfo("UserService-GetUserDropDown: Completed getting user drop down");
            }
            catch (Exception ex)
            {
                retVal = null;
                _logger.LogError("UserService-GetUserDropDown: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }

        public async Task<UsersModel> GetUser()
        {

            UsersModel usersModel = new UsersModel();
            var client = GraphClientUtility.GetGraphServiceClient();

            if (client == null)
            {
                return usersModel;
            }
            try
            {
                var userList = await client.Users.Request().GetAsync();

                if (userList != null)
                {
                    foreach (var user in userList)
                    {
                        UserModel userModel = GraphClientUtility.ConvertGraphUserToUserModel(user, null);
                        usersModel.Users.Add(userModel);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return usersModel;

        }

        public async Task<UsersModel> GetDeletedUser()
        {

            UsersModel usersModel = new UsersModel();
            var client = GraphClientUtility.GetGraphServiceClient();

            if (client == null)
            {
                return usersModel;
            }

            var delUserList = await client.Directory.DeletedItems["microsoft.graph.user"].Request().GetAsync();

            var delUsers = JArray.Parse(delUserList.AdditionalData["value"].ToString()).ToList();

            if (delUsers != null)
            {
                foreach (var user in delUsers)
                {
                    UserModel userModel = GraphClientUtility.ConvertDeletedGraphUserToUserModel(user, null);
                    usersModel.Users.Add(userModel);
                }
            }

            return usersModel;
        }

        public async Task SendInvite(InviteUser user)
        {

            GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();

            var invitation = new Invitation
            {
                InvitedUserEmailAddress = user.InvitedUserEmailAddress,
                InviteRedirectUrl = "http://localhost:57774",
                InvitedUserDisplayName = user.InvitedUserDisplayName,
                InvitedUserMessageInfo = new InvitedUserMessageInfo
                {
                    CustomizedMessageBody = user.CustomizedMessageBody
                },
                InvitedUserType = user.InvitedUserType,
                SendInvitationMessage = true,
                InvitedUser = user.InvitedUser,
                Status = user.Status,
                InviteRedeemUrl = user.InviteRedeemUrl
            };

            await client.Invitations
                  .Request()
                      .AddAsync(invitation);

        }

        public async Task Create(UserModel user)
        {

            #region setting properites

            user.DisplayName = $"{user.GivenName} {user.Surname}";

            user.AutoGeneratePassword = string.IsNullOrEmpty(user.Password) ? true : false;

            #endregion

            GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();


            var tenantId = GraphClientUtility.Domain;
            var b2cExtensionAppClientId = GraphClientUtility.b2cExtensionAppClientId;

            var newUser = BuildUserForCreation(user, tenantId);

            var createdUser =  await client.Users.Request().AddAsync(newUser);

            IDictionary<string, object> schemaExtensions = new Dictionary<string, object>();

            #region Custom Attributes

            var properties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
                          .Request()
                          .GetAsync();

            foreach (ExtensionProperty ext in properties.CurrentPage)
            {
                var names = ext.Name.Split('_');
                var additional = user.AdditionalData.Where(attribute => attribute.Key == names[names.Length - 1]).ToList();
                if (additional.Count > 0)
                {
                    schemaExtensions.Add(ext.Name, additional.FirstOrDefault().Value);
                }

            }

            #endregion

            var extension = await client.Users[createdUser.Id].Request().UpdateAsync(new User
            {
                AdditionalData = schemaExtensions

            });

            if (createdUser != null)
            {

                newUser.Id = createdUser.Id;

                #region Assign group(s) 

                if (user.Groups != null)
                {
                    if (user.Groups.Any())
                    {

                        AssignGroupsToUser(client, createdUser, user.Groups);

                    }
                }

                #endregion
            }

        }

        public async Task Update(UserModel user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.UserPrincipalName) && string.IsNullOrEmpty(user.Id))
                {
                    var errorMessage = $"Cannot update user. User id [{user.Id}] required field is missing.";
                    _logger.LogWarn(errorMessage);
                }

                var client = GraphClientUtility.GetGraphServiceClient();
                if (client == null)
                {
                    var errorMessage = $"[User Update] Unable to get the proxy for Graph client for user [{user.UserPrincipalName}]";
                    _logger.LogWarn(errorMessage);
                }

                var b2cExtensionAppClientId = GraphClientUtility.b2cExtensionAppClientId;

                var userService = new UserService(_logger);
                var updatingUser = userService.BuildUserForUpdate(user, b2cExtensionAppClientId);

                var userAttributeService = new UserAttributeService(_logger);
                var taskSchema = userAttributeService.GetIfExtensionExist(GraphClientUtility.ExtensionName);
                var schemaName = taskSchema.Result.Id;
                IDictionary<string, object> schemaExtensions = new Dictionary<string, object>();
                schemaExtensions.Add(schemaName + "_isActiveUser", true);

                //updatingUser.AdditionalData = schemaExtensions;
                // var result = await client.Users[user.Id].Extensions.Request().AddAsync(new Extension { AdditionalData = schemaExtensions });
                var result = await client.Users[user.Id].Request().UpdateAsync(updatingUser);


                #region Custom Attributes

                var properties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
                              .Request()
                              .GetAsync();

                foreach (ExtensionProperty ext in properties.CurrentPage)
                {
                    var names = ext.Name.Split('_');
                    var additional = user.AdditionalData.Where(attribute => attribute.Key == names[names.Length - 1]).ToList();
                    if (additional.Count > 0)
                    {
                        schemaExtensions.Add(ext.Name, additional.FirstOrDefault().Value);
                    }

                }

                #endregion

                var extension = await client.Users[result.Id].Request().UpdateAsync(new User
                {
                    AdditionalData = schemaExtensions

                });

                if (result == null)
                {
                    var errorMessage = $"Failed to update user for [{user.UserPrincipalName}]";
                    _logger.LogWarn(errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"User Update :error updating user for user id {user.Id}");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task<UsersModel> GetFilteredUsers(string filter)
        {
            UsersModel usersModel = new UsersModel();

            var client = GraphClientUtility.GetGraphServiceClient();
            var userList = await client.Users.Request().Filter($"startswith(displayName,'{filter}') or startswith(givenName,'{filter}') or startswith(surname,'{filter}') or startswith(mail,'{filter}') or startswith(userPrincipalName,'{filter}')").GetAsync();
            if (userList != null)
            {
                foreach (var user in userList)
                {
                    UserModel userModel = GraphClientUtility.ConvertGraphUserToUserModel(user, null);
                    usersModel.Users.Add(userModel);
                }
            }

            return usersModel;
        }

        public async Task<UserModel> GetUser(string id)
        {
            var userModel = new UserModel();
            try
            {
                var client = GraphClientUtility.GetGraphServiceClient();

                var extProperties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
                           .Request()
                           .GetAsync();

                var extnames = new List<string>();
                foreach (ExtensionProperty name in extProperties.CurrentPage)
                {
                    extnames.Add(name.Name);

                }

                var user = await client.Users[id]
                           .Request()
                           .GetAsync();

                //var user = await client.Users[id].Request().GetAsync();

                //var extensions = await client.Users[id].Extensions.Request().GetAsync();


                //user.Extensions = extensions;

                userModel = GraphClientUtility.ConvertGraphUserToUserModel(user, null);

                var schemaExtensions = new List<UserType>();
                var properties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
                          .Request()
                          .GetAsync();
                var userattributes = await client.Users[id]
                          .Request()
                           .Select(string.Join(",", extnames))
                          .GetAsync();
                foreach (ExtensionProperty ext in properties.CurrentPage)
                {
                    var names = ext.Name.Split('_');
                    var additional = userattributes.AdditionalData.Where(attribute => attribute.Key == ext.Name).ToList();
                    if (additional.Count > 0)
                    {
                        schemaExtensions.Add(new UserType
                        {
                            Key = names[names.Length - 1],
                            Value = additional.FirstOrDefault().Value.ToString()
                        });
                    }

                }
                userModel.AdditionalData = schemaExtensions;
                userModel.SignInName = user.UserPrincipalName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw ex;
            }

            return userModel;
        }

        #endregion

        #region Private Methods
        private Dictionary<string, object> BulidCustomExtension(string b2cExtensionAppClientId, Dictionary<string, string> customAttributes)
        {
            Dictionary<string, object> retVal = null;

            try
            {
                if (string.IsNullOrWhiteSpace(b2cExtensionAppClientId))
                {
                    _logger.LogError("B2C Extension App ClientId (ApplicationId) is missing in the appsettings.json. " +
                        "Get it from the App Registrations blade in the Azure portal. The app registration has the name 'b2c-extensions-app. " +
                        "Do not modify. Used by AADB2C for storing user data.");
                    return retVal;
                }

                if (customAttributes != null)
                {
                    var extensionAppClientId = b2cExtensionAppClientId.Replace(CareStreamConst.Dash, "");
                    retVal = new Dictionary<string, object>();

                    foreach (KeyValuePair<string, string> entry in customAttributes)
                    {
                        if (!string.IsNullOrEmpty(entry.Key))
                        {
                            var key = $"{CareStreamConst.Extension}_{extensionAppClientId}_{entry.Key}";
                            retVal.Add(key, entry.Value);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-BulidCustomExtension: Exception occurred...");
                _logger.LogError(ex);
            }

            return retVal;
        }

        private string getMailNickName(string givenName, string surName)
        {

            givenName = givenName != null ? givenName : "";
            surName = surName != null ? surName : "";

            if (string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(surName))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(givenName) && !string.IsNullOrEmpty(surName))
            {
                return (surName.Replace(" ", "")).Trim();
            }

            if (!string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(surName))
            {
                return (givenName.Replace(" ", "")).Trim();
            }

            return (givenName.Replace(" ", "") + "." + surName.Replace(" ", "")).Trim();
        }

        private int GetRandomNumber()
        {
            var retVal = 4;
            try
            {
                Random random = new Random();
                retVal = random.Next(1, 10);
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-GetRandomString: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        private List<UserRole> GetUserRoles()
        {
            List<UserRole> retVal = null;
            try
            {
                retVal = new List<UserRole>
                {
                    new UserRole
                    {
                        Key = "AppAdm",
                        Value = "Application administrator"
                    },
                     new UserRole
                    {
                        Key = "AppDev",
                        Value = "Application developer"
                    },
                      new UserRole
                    {
                        Key = "AutAdm",
                        Value = "Authentication administrator"
                    },
                     new UserRole
                    {
                        Key = "AzDevAdm",
                        Value = "Azure DevOps administrator"
                    },

                    new UserRole
                    {
                        Key = "AzInfProAdm",
                        Value = "Azure Information Protection administrator"
                    },
                     new UserRole
                    {
                        Key = "B2CIEFAdm",
                        Value = "B2C IEF Keyset administrator"
                    },
                      new UserRole
                    {
                        Key = "ClDevAdm",
                        Value = "Cloud device administrator"
                    },
                     new UserRole
                    {
                        Key = "DeskAnaAdm",
                        Value = "Desktop Analytics administrator"
                    },
                        new UserRole
                    {
                        Key = "GloAdm",
                        Value = "Global administrator"
                    },
                     new UserRole
                    {
                        Key = "GloRea",
                        Value = "Global reader"
                    }
                };

                retVal = retVal.OrderBy(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-GetUserRoles: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        private List<UserType> GetUserTypes()
        {
            List<UserType> retVal = null;
            try
            {
                retVal = new List<UserType>
                {
                    new UserType
                    {
                        Key = "Cus",
                        Value = "Customer"
                    },
                    new UserType
                    {
                        Key = "Del",
                        Value = "Dealer"
                    },
                     new UserType
                    {
                        Key = "Con",
                        Value = "Contractor"
                    },
                    new UserType
                    {
                        Key = "Ext",
                        Value = "External Manager"
                    },
                    new UserType
                    {
                        Key = "Oth",
                        Value = "Other"
                    }
                };

                retVal = retVal.OrderBy(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-GetUserRoles: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        private List<UserLanguage> GetUserLanguage()
        {
            List<UserLanguage> retVal = null;
            try
            {
                retVal = new List<UserLanguage>
                {
                    new UserLanguage
                    {
                        Key = "Ar",
                        Value = "Arabic"
                    },
                     new UserLanguage
                    {
                        Key = "EN",
                        Value = "English"
                    },
                      new UserLanguage
                    {
                        Key = "GE",
                        Value = "German"
                    },
                     new UserLanguage
                    {
                        Key = "FR",
                        Value = "French"
                    },
                      new UserLanguage
                    {
                        Key = "RU",
                        Value = "Russian"
                    },
                     new UserLanguage
                    {
                        Key = "SP",
                        Value = "Spanish"
                    }

                };

                retVal = retVal.OrderBy(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-GetUserLanguage: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        private List<UserBusinessDepartment> GetUserBusinessDepartments()
        {
            List<UserBusinessDepartment> retVal = null;
            try
            {
                retVal = new List<UserBusinessDepartment>
                {
                    new UserBusinessDepartment
                    {
                        Key = "Ser",
                        Value = "Service"
                    },
                     new UserBusinessDepartment
                    {
                        Key = "Mar",
                        Value = "Marketing"
                    },
                     new UserBusinessDepartment
                    {
                        Key = "Sal",
                        Value = "Sales"
                    },
                     new UserBusinessDepartment
                    {
                        Key = "Pur",
                        Value = "Purchasing"
                    },
                    new UserBusinessDepartment
                    {
                        Key = "Fin",
                        Value = "Finance"
                    },
                     new UserBusinessDepartment
                    {
                        Key = "Log",
                        Value = "Logistics"
                    }
                };

                retVal = retVal.OrderBy(x => x.Value).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserService-GetUserBusinessDepartments: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        private List<UserType> GetUserAttributes()
        {
            return new List<UserType>
                   {
                        new UserType
                        {
                            Key = "Customer Name",
                            Value = "String"
                        },
                        new UserType
                        {
                            Key = "Is Customer Satisfied",
                            Value = "Boolean"
                        },
                         new UserType
                        {
                            Key = "Customer Salary",
                            Value = "Integer"
                        }
                  };
        }

        #endregion

    }
}
