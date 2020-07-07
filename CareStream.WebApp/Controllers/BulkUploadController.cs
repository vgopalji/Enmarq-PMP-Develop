using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Scheduler;
using CareStream.Utility;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog.Fluent;

namespace CareStream.WebApp.Controllers
{
    public class BulkUploadController : BaseController
    {
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ILoggerManager _logger;
        private readonly CareStreamContext _cc;
        private readonly IUserService _userService;

        [Obsolete]
        public BulkUploadController(IHostingEnvironment hostingEnvironment, CareStreamContext cc,
            IUserService userService, ILoggerManager logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _cc = cc;
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Upload(string id)
        {
            try
            {
                var str = string.IsNullOrEmpty(id) ? string.Empty : id;
                TempData[CareStreamConst.Bulk_Action] = str;
            }
            catch (Exception ex)
            {
                _logger.LogError("BulkUploadController-Upload: Exception occurred...");
                _logger.LogError(ex);
            }

            return View();
        }

        [HttpGet]
        [Obsolete]
        public IActionResult Download(string id)
        {
            try
            {
                var fileName = GetTemplateFileName(id);

                var folderPath = Path.Combine(_hostingEnvironment.WebRootPath, "Template");
                var path = Path.Combine(folderPath, fileName);
                var fs = new FileStream(path, FileMode.Open);

                return File(fs, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError("BulkUploadController-Download: Exception occurred...");
                _logger.LogError(ex);
                return null;
            }
        }

        public async Task<IActionResult> DownloadUsers()
        {
            var usersModel = await _userService.GetUser();

            var builder = new StringBuilder();
            builder.AppendLine("DisplayName,UserPrincipalName,InitialPassword,BlockSignIn,FirstName,LastName,JobTitle,Department,Usagelocation,StreetAddress,State,Country,Office,City,ZIP,OfficePhone,MobilePhone");
            foreach (var user in usersModel.Users)
            {
                builder.AppendLine($"{user.DisplayName},{user.UserPrincipalName},{user.Password},'',{user.GivenName},{user.Surname}, {user.JobTitle}, {user.Department}, {user.UsageLocation}, {user.StreetAddress}, {user.State},{user.Country}, {user.OfficeLocation}, {user.City}, {user.PostalCode}, {user.BusinessPhone}, {user.MobilePhone}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "users.csv");
        }

        [HttpPost("FileUpload")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(List<IFormFile> files, string id)
        {
            try
            {
                BulkUserFile bulkUserFile = null;
                List<BulkUser> bulkUsers = null;

                ClaimsPrincipal currentUser = this.User;
                var actionFor = string.IsNullOrEmpty(id) ? CareStreamConst.Bulk_User_Create : id;

                foreach (var formFile in files)
                {
                    try
                    {
                        if (formFile.Length > 0)
                        {
                            bulkUserFile = new BulkUserFile();

                            var filePath = Path.GetTempFileName();

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }

                            TextReader reader = new StreamReader(filePath);
                            var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                            csv.Read();
                            csv.ReadHeader();

                            bulkUserFile.Action = actionFor;
                            bulkUserFile.CreatedDate = DateTime.Now;
                            bulkUserFile.FileName = formFile.FileName;
                            bulkUserFile.FileSize = formFile.Length.ToString();
                            bulkUserFile.Status = CareStreamConst.Bulk_User_Loaded_Status;
                            bulkUserFile.UploadBy = string.IsNullOrEmpty(currentUser.Identity.Name) ? CareStreamConst.Bulk_User_UploadedBy : currentUser.Identity.Name;

                            bulkUsers = ProcessCSVAndCreateDbObject(csv, actionFor);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"BulkUploadController-Index: Error reading the file name {formFile.FileName} ");
                        _logger.LogError(ex);
                    }

                }

                if (bulkUserFile != null)
                {
                    if (IsBulkUserFileValid(bulkUserFile))
                    {
                        try
                        {
                            _cc.Add(bulkUserFile);
                            var result = _cc.SaveChanges();

                            if (result > 0)
                            {
                                var fileId = bulkUserFile.Id;
                                if (bulkUsers != null)
                                {
                                    if (bulkUsers.Any())
                                    {
                                        foreach (var bulkUser in bulkUsers)
                                        {
                                            try
                                            {
                                                bulkUser.FileId = fileId;
                                                if (IsBulkUserValid(bulkUser, actionFor))
                                                {
                                                    _cc.Add(bulkUser);
                                                    var bulkUserResult = _cc.SaveChanges();

                                                    if (bulkUserResult == 0)
                                                    {
                                                        _logger.LogWarn($"BulkUploadController-Index: Unable to save bulk user for action {bulkUser.Action} and file id {fileId}");
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                _logger.LogError($"BulkUploadController-Index: Error saving the user detail for action {bulkUser.Action}");
                                                _logger.LogError(ex);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"BulkUploadController-Index: Error saving the file name {bulkUserFile.FileName} ");
                            _logger.LogError(ex);
                        }
                    }
                }

                ShowSuccessMessage($"File has been uploaded successfully. We will notify you once the operation {actionFor} is completed");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("File upload failed: " + ex.Message);
                _logger.LogError("BulkUploadController-Index: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction(nameof(Upload), new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> Process()
        {
            try
            {
                UserScheduleProcessor userScheduleProcessor = new UserScheduleProcessor();
                await userScheduleProcessor.Process();
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction(nameof(Upload), new { id = "Create" });
        }

        private List<BulkUser> ProcessCSVAndCreateDbObject(CsvReader csvReader, string actionFor)
        {
            List<BulkUser> bulkUsers = new List<BulkUser>();
            try
            {
                if (csvReader != null)
                {
                    switch (actionFor)
                    {
                        case CareStreamConst.Bulk_User_Create:
                        case CareStreamConst.Bulk_User_Update:

                            #region Create || Update User

                            while (csvReader.Read())
                            {
                                BulkUser bulkUser = new BulkUser
                                {
                                    Action = actionFor,
                                    Status = CareStreamConst.Bulk_User_Loaded_Status,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow
                                };

                                foreach (string header in csvReader.Context.HeaderRecord)
                                {
                                    try
                                    {

                                        switch (header.ToLower())
                                        {
                                            case CareStreamConst.BU_DisplayName:
                                                bulkUser.DisplayName = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_UserPrincipalName:
                                                bulkUser.UserPrincipalName = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_InitialPassword:
                                                bulkUser.InitialPassword = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_BlockSignIn:
                                                var blockSignStr = csvReader.GetField(header);
                                                if (string.IsNullOrEmpty(blockSignStr))
                                                {
                                                    bulkUser.BlockSignIn = false;
                                                }
                                                else if (blockSignStr.ToLower() == "yes")
                                                {
                                                    bulkUser.BlockSignIn = true;
                                                }
                                                else
                                                {
                                                    bulkUser.BlockSignIn = false;
                                                }
                                                break;
                                            case CareStreamConst.BU_FirstName:
                                                bulkUser.FirstName = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_LastName:
                                                bulkUser.LastName = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_JobTitle:
                                                bulkUser.JobTitle = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_Department:
                                                bulkUser.Department = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_Usagelocation:
                                                bulkUser.Usagelocation = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_StreetAddress:
                                                bulkUser.StreetAddress = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_State:
                                                bulkUser.State = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_Country:
                                                bulkUser.Country = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_Office:
                                                bulkUser.Office = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_City:
                                                bulkUser.City = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_ZIP:
                                                bulkUser.ZIP = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_OfficePhone:
                                                bulkUser.OfficePhone = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_MobilePhone:
                                                bulkUser.MobilePhone = csvReader.GetField(header);
                                                break;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"BulkUploadController-ProcessCSVAndCreateDbObject: Create || Update user error reading values for header- {header}");
                                        _logger.LogError(ex);
                                    }

                                }

                                bulkUsers.Add(bulkUser);
                            }

                            #endregion

                            break;
                        case CareStreamConst.Bulk_User_Invite:

                            #region Invite User

                            while (csvReader.Read())
                            {
                                BulkUser bulkUser = new BulkUser
                                {
                                    Action = actionFor,
                                    Status = CareStreamConst.Bulk_User_Loaded_Status,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow
                                };

                                foreach (string header in csvReader.Context.HeaderRecord)
                                {
                                    try
                                    {

                                        switch (header.ToLower())
                                        {
                                            case CareStreamConst.BU_InviteeEmail:
                                                bulkUser.InviteeEmail = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_InviteRedirectURL:
                                                bulkUser.InviteRedirectURL = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_CustomizedMessageBody:
                                                bulkUser.CustomizedMessageBody = csvReader.GetField(header);
                                                break;
                                            case CareStreamConst.BU_SendEmail:
                                                var blockSignStr = csvReader.GetField(header);
                                                if (string.IsNullOrEmpty(blockSignStr))
                                                {
                                                    bulkUser.SendEmail = false;
                                                }
                                                else if (blockSignStr.ToLower() == "true")
                                                {
                                                    bulkUser.SendEmail = true;
                                                }
                                                else
                                                {
                                                    bulkUser.SendEmail = false;
                                                }
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"BulkUploadController-ProcessCSVAndCreateDbObject: Invite user error reading values for header- {header}");
                                        _logger.LogError(ex);
                                    }
                                }
                                bulkUsers.Add(bulkUser);
                            }
                            #endregion

                            break;
                        case CareStreamConst.Bulk_User_Delete:

                            #region Delete User

                            while (csvReader.Read())
                            {
                                BulkUser bulkUser = new BulkUser
                                {
                                    Action = actionFor,
                                    Status = CareStreamConst.Bulk_User_Loaded_Status,
                                    CreatedDate = DateTime.UtcNow,
                                    ModifiedDate = DateTime.UtcNow
                                };
                                foreach (string header in csvReader.Context.HeaderRecord)
                                {
                                    try
                                    {
                                        switch (header.ToLower())
                                        {
                                            case CareStreamConst.BU_UserPrincipalName:
                                                bulkUser.UserPrincipalName = csvReader.GetField(header);
                                                break;

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"BulkUploadController-ProcessCSVAndCreateDbObject: Delete user error reading values for header- {header}");
                                        _logger.LogError(ex);
                                    }

                                }
                                bulkUsers.Add(bulkUser);
                            }
                            #endregion

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("BulkUploadController-ProcessCSVAndCreateDbObject: Exception occurred...");
                _logger.LogError(ex);
            }
            return bulkUsers;
        }

        private bool IsBulkUserFileValid(BulkUserFile bulkUserFile)
        {
            var retVal = true;
            try
            {
                if (bulkUserFile == null)
                    return false;

                if (string.IsNullOrEmpty(bulkUserFile.FileName))
                    return false;

                if (string.IsNullOrEmpty(bulkUserFile.FileSize))
                    return false;

                if (string.IsNullOrEmpty(bulkUserFile.Action))
                    return false;
            }
            catch (Exception ex)
            {
                retVal = false;
                _logger.LogError("BulkUploadController-IsBulkUserFileValid: Exception occurred...");
                _logger.LogError(ex);
            }

            return retVal;
        }

        private bool IsBulkUserValid(BulkUser bulkUser, string action)
        {
            var retVal = true;
            try
            {
                if (bulkUser == null || string.IsNullOrEmpty(action))
                    return false;

                switch (action)
                {
                    case CareStreamConst.Bulk_User_Create:
                    case CareStreamConst.Bulk_User_Update:

                        #region Create || Update
                        if (bulkUser.FileId == 0)
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.Action))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.UserPrincipalName))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.DisplayName))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.InitialPassword))
                        {
                            return false;
                        }

                        #endregion

                        break;
                    case CareStreamConst.Bulk_User_Invite:

                        #region Invite
                        if (bulkUser.FileId == 0)
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.Action))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.InviteeEmail))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.InviteRedirectURL))
                        {
                            return false;
                        }


                        #endregion
                        break;

                    case CareStreamConst.Bulk_User_Delete:

                        #region Create || Update
                        if (bulkUser.FileId == 0)
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.Action))
                        {
                            return false;
                        }

                        if (string.IsNullOrEmpty(bulkUser.UserPrincipalName))
                        {
                            return false;
                        }


                        #endregion
                        break;
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                _logger.LogError("BulkUploadController-IsBulkUserValid: Exception occurred...");
                _logger.LogError(ex);
            }

            return retVal;
        }

        private string GetTemplateFileName(string action)
        {
            string fileName;
            switch (action)
            {
                case CareStreamConst.Bulk_User_Create:
                    fileName = "UserCreateTemplate.csv";
                    break;
                case CareStreamConst.Bulk_User_Invite:
                    fileName = "UserInviteTemplate.csv";
                    break;
                case CareStreamConst.Bulk_User_Update:
                    fileName = "UserUpdateTemplate.csv";
                    break;
                case CareStreamConst.Bulk_User_Delete:
                    fileName = "UserDeleteTemplate.csv";
                    break;
                default:
                    fileName = "Template.csv";
                    break;
            }
            return fileName;
        }
    }
}