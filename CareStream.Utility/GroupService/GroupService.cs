using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Utility
{
    public interface IGroupService
    {
        Task<List<GroupModel>> GetDetailGroupList();

        Task<GroupModel> GetGroup(string id);

        Task<List<GroupAssignModel>> BuildGroupOwnerMember();

        Task<GroupModel> CreateGroup(GroupModel groupModel);

        Task RemoveGroup(List<string> groupIdsToDelete);

        Dictionary<string, string> GetGroups();
    }

    public class GroupService : IGroupService
    {
        private readonly ILoggerManager _logger;
        private IGroupOwnerService _groupOwnerService;
        private IGroupMemberService _groupMemberService;

        public GroupService(ILoggerManager logger, IGroupOwnerService groupOwnerService, IGroupMemberService groupMemberService)
        {
            _logger = logger;
            _groupOwnerService = groupOwnerService;
            _groupMemberService = groupMemberService;
        }

        public GroupService()
        {
            _logger = new LoggerManager();
        }

        #region MVC Controller Helpers

        public async Task<List<GroupAssignModel>> BuildGroupOwnerMember()
        {
            var groupAssignModel = new List<GroupAssignModel>();
            try
            {
                _logger.LogInfo("GroupService-BuildGroupOwnerMember: [Started] to get detail list of user to build group owner and member");

               var _graphServiceClient = GraphClientUtility.GetGraphServiceClient();

                if (_graphServiceClient == null)
                {
                    _logger.LogError("GroupService-BuildGroupOwnerMember: Unable to create object for graph client");
                    return null;
                }

                var taskGroupOwnerMember = await GetNewGroupDefaultOwnerMember();

                if (taskGroupOwnerMember != null)
                {
                    if (taskGroupOwnerMember.Any())
                    {
                        var ownerAssignModel = new GroupAssignModel
                        {
                            AssignFor = CareStreamConst.Owner
                        };
                        var memberAssignModel = new GroupAssignModel
                        {
                            AssignFor = CareStreamConst.Member
                        };

                        foreach (var aDict in taskGroupOwnerMember.OrderBy(x => x.Value))
                        {
                            ownerAssignModel.AssignList.Add(aDict.Key, aDict.Value);
                            memberAssignModel.AssignList.Add(aDict.Key, aDict.Value);
                        }

                        groupAssignModel.Add(ownerAssignModel);
                        groupAssignModel.Add(memberAssignModel);
                    }

                }

                _logger.LogInfo("GroupService-BuildGroupOwnerMember: [Completed] to getting detail list of user to build group owner and member");

            }
            catch (ServiceException ex)
            {
                _logger.LogError("GroupService-BuildGroupOwnerMember: Exception occured....");
                _logger.LogError(ex);
            }
            return groupAssignModel;
        }

        public async Task<List<GroupModel>> GetDetailGroupList()
        {
            try
            {

                var _graphServiceClient = GraphClientUtility.GetGraphServiceClient();

                if (_graphServiceClient == null)
                {
                    return null;
                }

                var groupsModel = await GetGroupDetails();

                return groupsModel;
            }
            catch (ServiceException ex)
            {
                _logger.LogError("GroupService-GetDetailGroupList: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task<GroupModel> GetGroup(string id)
        {
            try
            {
                var groupModel = new GroupModel();

                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                {
                    _logger.LogError("GroupService-GetGroup: Id cannot be empty");
                    return null;
                }

                _logger.LogInfo($"GroupService-GetGroup: [Started] to get detail for group id {id} from Azure AD B2C");

                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("GroupService-GetGroup: Unable to create object for graph client");
                    return null;
                }

                var group = await client.Groups[id].Request().GetAsync();

                if (group != null)
                {
                    _logger.LogInfo($"GroupService-GetGroup: Fetched the group detail for id {id} from Azure AD B2C");
                    groupModel = GraphClientUtility.ConvertGraphGroupToGroupModel(group, _logger);

                    var members = await client.Groups[id].Members.Request().Select(e => new { e.Id }).GetAsync();
                    if (members != null)
                    {
                        groupModel.NoOfMembers = members.Count();
                    }

                    var owners = await client.Groups[id].Owners.Request().Select(e => new { e.Id }).GetAsync();
                    if (owners != null)
                    {
                        groupModel.NoOfOwners = owners.Count();
                    }
                }


                _logger.LogInfo($"GroupService-GetGroup: [Completed] to get detail for group id {id} from Azure AD B2C");
                return groupModel;

            }
            catch (ServiceException ex)
            {
                _logger.LogError("GroupService-GetGroup: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task<GroupModel> CreateGroup(GroupModel groupModel)
        {
            try
            {
                _logger.LogInfo("GroupService-CreateGroup: [Started] creation of group in Azure AD B2C");

                if (groupModel == null)
                {
                    _logger.LogError("GroupService-CreateGroup: Group Model cannot be null...");
                    return null;
                }

                var client = GraphClientUtility.GetGraphServiceClient();
                if (client == null)
                {
                    _logger.LogError("GroupService-CreateGroup: Unable to create object for graph client ");
                    return null;
                }

                var group = BuildGroup(groupModel);

                var newGroup = await client.Groups.Request().AddAsync(group);

                var newGroupModel = GraphClientUtility.ConvertGraphGroupToGroupModel(newGroup, _logger);

                if (newGroupModel != null)
                {
                    var groupId = newGroup.Id;
                    if (!string.IsNullOrEmpty(groupId))
                    {
                        #region Assign Members

                        if (groupModel.OwnerSelected.Any())
                        {
                            _logger.LogInfo($"GroupService-CreateGroup: assigning {groupModel.OwnerSelected.Count()} owner(s) for group {newGroupModel.DisplayName} in Azure AD B2C");

                            var groupOwnerAssign = new GroupOwnerAssignModel
                            {
                                GroupId = groupId,
                                SelectedOwners = groupModel.OwnerSelected
                            };

                            var groupOwnerService = new GroupOwnerService(_logger);
                            await groupOwnerService.AddGroupOwners(groupOwnerAssign);

                            _logger.LogInfo($"GroupService-CreateGroup: assigned {groupModel.OwnerSelected.Count()} owner(s) for group {newGroupModel.DisplayName} in Azure AD B2C");
                        }

                        if (groupModel.MemberSelected.Any())
                        {
                            _logger.LogInfo($"GroupService-CreateGroup: assigning {groupModel.MemberSelected.Count()} member(s) for group {newGroupModel.DisplayName} in Azure AD B2C");

                            var groupMemberAssign = new GroupMemberAssignModel
                            {
                                GroupId = groupId,
                                SelectedMembers = groupModel.MemberSelected
                            };

                            var groupMemberService = new GroupMemberService(null);
                            await groupMemberService.AddGroupMembers(groupMemberAssign);

                            _logger.LogInfo($"GroupService-CreateGroup: assigned {groupModel.MemberSelected.Count()} member(s) for group {newGroupModel.DisplayName} in Azure AD B2C");
                        }
                        #endregion
                    }

                }

                _logger.LogInfo("GroupService-CreateGroup: [Completed] creation of group in Azure AD B2C");
                return newGroupModel;

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-CreateGroup: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task RemoveGroup(List<string> groupIdsToDelete)
        {
            try
            {
                if (groupIdsToDelete == null)
                {
                    _logger.LogError("GroupService-RemoveGroup: Input value cannot be empty");
                    return;
                }

                GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();
                if (client == null)
                {
                    _logger.LogError("GroupService-RemoveGroup: Unable to create object for graph client");
                    return;
                }

                foreach (var id in groupIdsToDelete)
                {
                    try
                    {
                        _logger.LogInfo($"GroupService-RemoveGroup: [Started] removing group for id [{id}] on Azure AD B2C");

                        await client.Groups[id].Request().DeleteAsync();

                        _logger.LogInfo($"GroupService-RemoveGroup: [Completed] removed group [{id}] on Azure AD B2C");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"GroupService-RemoveGroup: Exception occured while removing group for id [{id}]");
                        _logger.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-RemoveGroup: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        #endregion

        #region API Calls

        public Dictionary<string, string> GetGroups()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            try
            {
                var tasks = FetchGroups();
                foreach (var aDict in tasks.Result)
                {
                    retVal.Add(aDict.Key, aDict.Value);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-GetGroups: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }

        public async Task<List<GroupModel>> GetGroupDetails()
        {
            List<GroupModel> retVal = new List<GroupModel>();
            try
            {
                _logger.LogInfo("GroupService-GetGroups: Getting group detail from Azure AD B2C");

                var _graphServiceClient = GraphClientUtility.GetGraphServiceClient();
                var groupDetails = await _graphServiceClient.Groups.Request().GetAsync();

                if (groupDetails != null)
                {
                    foreach (var groupItem in groupDetails)
                    {
                        try
                        {

                            var groupModel = GraphClientUtility.ConvertGraphGroupToGroupModel(groupItem, _logger);

                            if (groupItem.GroupTypes != null)
                            {
                                if (groupItem.GroupTypes.Any())
                                {
                                    groupModel.GroupType = string.Join('|', groupItem.GroupTypes);
                                }
                            }
                            retVal.Add(groupModel);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"GroupService-GetGroups: failed to add group for group name {groupItem.DisplayName}");
                            _logger.LogError(ex);
                            throw ex;
                        }

                    }
                    retVal = retVal.OrderBy(x => x.DisplayName).ToList();
                }

                _logger.LogInfo("GroupService-GetGroups: Completed getting the group detail from Azure AD B2C");
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-GetGroups: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }

        public Group BuildGroup(GroupModel groupModel)
        {
            Group retVal = null;
            try
            {
                _logger.LogInfo($"GroupService-BuildGroup: Building group Object for {groupModel.DisplayName}");
                if (groupModel == null)
                {
                    _logger.LogError("GroupService-BuildGroup: Input value cannot be null");
                    return retVal;
                }

                retVal = new Group
                {
                    MailNickname = groupModel.MailNickname,
                    DisplayName = groupModel.DisplayName,
                    Visibility = groupModel.Visibility,
                    MailEnabled = groupModel.MailEnabled,
                    SecurityEnabled = groupModel.SecurityEnabled,
                    GroupTypes = groupModel.GroupTypes,
                    Description = groupModel.Description,
                };

                //owner and members
                if (groupModel.AdditionalData != null)
                {
                    if (groupModel.AdditionalData.Count > 0)
                    {
                        retVal.AdditionalData = groupModel.AdditionalData;
                    }
                }


                _logger.LogInfo($"GroupService-BuildGroup: Completed building group Object for {groupModel.DisplayName}");
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-BuildGroup: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }

            return retVal;
        }

        public bool IsGroupModelValid(GroupModel groupModel)
        {
            if (groupModel == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(groupModel.DisplayName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(groupModel.MailNickname))
            {
                return false;
            }

            return true;
        }

        #endregion


        #region Private Methods

        private async Task<IDictionary<string, string>> FetchGroups()
        {
            Dictionary<string, string> retVal = null;
            try
            {
                _logger.LogInfo("FetchGroups-GetGroups: Getting group detail from Azure AD B2C");

                var _graphServiceClient = GraphClientUtility.GetGraphServiceClient();
                var groupList = await _graphServiceClient.Groups.Request().Select(e => new { e.DisplayName, e.Id }).GetAsync();

                if (groupList != null)
                {
                    _logger.LogInfo($"Found {groupList.Count} group from Azure AD B2C");
                    retVal = new Dictionary<string, string>();
                    foreach (var group in groupList.OrderBy(x => x.DisplayName))
                    {
                        if (!retVal.ContainsKey(group.DisplayName))
                        {
                            retVal.Add(group.DisplayName, group.Id);
                        }
                    }
                }

                _logger.LogInfo("FetchGroups-GetGroups: Completed getting the group detail from Azure AD B2C");
            }
            catch (Exception ex)
            {
                _logger.LogError("FetchGroups-GetGroups: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
            return retVal;
        }

        private async Task<Dictionary<string, string>> GetNewGroupDefaultOwnerMember()
        {
            Dictionary<string, string> retVal = null;
            try
            {
                var _graphServiceClient = GraphClientUtility.GetGraphServiceClient();
                var users = await _graphServiceClient.Users.Request().GetAsync();

                if (users != null)
                {
                    retVal = new Dictionary<string, string>();
                    foreach (var user in users.Where(x => !string.IsNullOrEmpty(x.UserPrincipalName)))
                    {
                        try
                        {
                            if (!retVal.ContainsKey(user.Id))
                            {
                                var value = string.IsNullOrEmpty(user.GivenName + user.Surname) ? user.UserPrincipalName : $"{user.GivenName} {user.Surname}";
                                retVal.Add(user.Id, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"GroupService-GetGroupOwnerMemberAsync: unable to add user in the group owner and member collection for user {user.UserPrincipalName}");
                            _logger.LogError(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-GetNewGroupDefaultOwnerMember: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        #endregion

    }
}
