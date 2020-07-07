using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CareStream.Scheduler
{
    public class UserCreate : IUserProcess
    {
        private readonly ILoggerManager _logger;
        private readonly CareStreamContext _cc;
        private readonly DbHelper _dbHelper;
        public UserCreate(ILoggerManager logger, CareStreamContext cc)
        {
            _logger = logger;
            _cc = cc;
            _dbHelper = new DbHelper(logger, cc);
        }

        public async Task ProcessUser(List<BulkUser> bulkUsers)
        {
            var logConst = "UserCreate-ProcessUser";
            try
            {
                foreach (var bulkUser in bulkUsers)
                {
                    try
                    {
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Started_Status);

                        #region UserModel

                        var userModel = new UserModel
                        {
                            AccountEnabled = bulkUser.BlockSignIn
                        };

                        if (!string.IsNullOrEmpty(bulkUser.UserPrincipalName))
                        {
                            userModel.SignInName = bulkUser.UserPrincipalName;
                            userModel.UserPrincipalName = bulkUser.UserPrincipalName;
                        }

                        if (!string.IsNullOrEmpty(bulkUser.DisplayName))
                            userModel.DisplayName = bulkUser.DisplayName;

                        if (!string.IsNullOrEmpty(bulkUser.InitialPassword))
                            userModel.Password = bulkUser.InitialPassword;

                        if (!string.IsNullOrEmpty(bulkUser.FirstName))
                            userModel.GivenName = bulkUser.FirstName;

                        if (!string.IsNullOrEmpty(bulkUser.LastName))
                            userModel.Surname = bulkUser.LastName;

                        if (!string.IsNullOrEmpty(bulkUser.JobTitle))
                            userModel.JobTitle = bulkUser.JobTitle;

                        if (!string.IsNullOrEmpty(bulkUser.Department))
                            userModel.Department = bulkUser.Department;

                        if (!string.IsNullOrEmpty(bulkUser.Usagelocation))
                            userModel.UsageLocation = bulkUser.Usagelocation;

                        if (!string.IsNullOrEmpty(bulkUser.StreetAddress))
                            userModel.StreetAddress = bulkUser.StreetAddress;

                        if (!string.IsNullOrEmpty(bulkUser.State))
                            userModel.State = bulkUser.State;

                        if (!string.IsNullOrEmpty(bulkUser.Country))
                            userModel.Country = bulkUser.Country;

                        if (!string.IsNullOrEmpty(bulkUser.Office))
                            userModel.OfficeLocation = bulkUser.Office;

                        if (!string.IsNullOrEmpty(bulkUser.City))
                            userModel.City = bulkUser.City;

                        if (!string.IsNullOrEmpty(bulkUser.ZIP))
                            userModel.PostalCode = bulkUser.ZIP;

                        if (!string.IsNullOrEmpty(bulkUser.MobilePhone))
                            userModel.MobilePhone = bulkUser.MobilePhone;

                        if (!string.IsNullOrEmpty(bulkUser.OfficePhone))
                        {
                            userModel.BusinessPhone = bulkUser.OfficePhone;
                        }
                        #endregion

                        var client = GraphClientUtility.GetGraphServiceClient();
                        if (client == null)
                        {
                            var errorMessage = $"[User Create] Unable to get the proxy for Graph client for user [{bulkUser.UserPrincipalName}]";
                            _logger.LogWarn(errorMessage);
                            continue;
                        }

                        var userService = new UserService(_logger);
                        if (!userService.IsUserModelValid(userModel))
                        {
                            var errorMessage = $"[Cannot created the user] For user [{bulkUser.UserPrincipalName}] required field for creation is missing, please correct the record.";

                            _logger.LogWarn(errorMessage);
                            bulkUser.Error = errorMessage;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                            continue;
                        }

                        var tenantId = GraphClientUtility.TenantId;
                        var b2cExtensionAppClientId = GraphClientUtility.b2cExtensionAppClientId;

                        var newUser = userService.BuildUserForCreation(userModel, GraphClientUtility.Domain);
                        newUser.UserPrincipalName = $"{newUser.MailNickname}@{GraphClientUtility.Domain}";

                        var result = await client.Users.Request().AddAsync(newUser);

                        if (result != null)
                        {
                            bulkUser.ObjectID = result.Id;
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Completed_Status);
                        }
                        else
                        {
                            bulkUser.Error = $"Failed to create user for [{bulkUser.UserPrincipalName}]";
                            _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"{logConst}:error creating user for bulk user id {bulkUser.Id}");
                        _logger.LogError(ex);

                        bulkUser.Error = $"{ex.ToString()}. Message: {ex.Message}";
                        _dbHelper.UpdateTable(bulkUser, CareStreamConst.Bulk_User_Failed_Status);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{logConst}:Exception occured while processing the bulk user...");
                _logger.LogError(ex);
            }
        }


    }
}
