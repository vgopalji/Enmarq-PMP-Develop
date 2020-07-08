using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using CareStream.Utility.DealerService;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace CareStream.WebApp.Controllers
{
    public class DealerController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IToastNotification _toastNotification;
        private readonly IDealerService _dealerService;
        private readonly IProductFamilyService _productFamilyService;
        public DealerController(ILoggerManager logger, IToastNotification toastNotification, IDealerService dealerService, IProductFamilyService productFamilyService)
        {
            _logger = logger;
            _toastNotification = toastNotification;
            _dealerService = dealerService;
            _productFamilyService = productFamilyService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dealers = await _dealerService.GetAsync();
                return View(dealers);
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("Index", new List<DealerModel>());
            }

        }

        public async Task<IActionResult> Create()
        {
            DealerModel dm = new DealerModel();
            var productFamily = await _productFamilyService.getAllProductFamily();
            try
            {
                dm.productFamilyModels = productFamily;
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Create: Exception occurred...");
                _logger.LogError(ex);
                return View("create", new DealerModel());
            }
            return View(dm);

        }

        public async Task<IActionResult> Deleted()
        {
            try
            {
                var deletedDealer = await _dealerService.GetDeletedDealers();
                return View(deletedDealer);
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("Deleted", new List<DeletedDealerModel>());
            }


        }

        //public void AddSelectedDealers([FromBody] DealerAssignedModel dealerAssignedModel)
        //{
        //    if (dealerAssignedModel != null)
        //    {
        //        if (dealerAssignedModel.SelectedDealers != null)// && !string.IsNullOrEmpty(dealerAssignedModel.SelectedDealers[0].dea
        //        {
        //            TempData[CareStreamConst.Dealer] = dealerAssignedModel.SelectedDealers;

        //        }

        //    }
        //}

        public async Task<IActionResult> Post([FromForm] DealerModel dealerModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", dealerModel);
                }

                if (dealerModel != null)
                {
                    if (string.IsNullOrEmpty(dealerModel.DealerDescription) || string.IsNullOrEmpty(dealerModel.DealerName))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                DealerModel dmcList = new DealerModel();
                List<AssignedProductFamilyModel> apfl = new List<AssignedProductFamilyModel>();

                if (TempData[CareStreamConst.DealerProductFamily] != null)
                {
                    if (TempData[CareStreamConst.DealerProductFamily] is string[])
                    {
                        var selectedProductFamily = TempData[CareStreamConst.DealerProductFamily] as string[];
                        if (selectedProductFamily != null)
                        {
                            List<string> list = new List<string>(selectedProductFamily);
                            foreach (var res in list)
                            {
                                AssignedProductFamilyModel apf = new AssignedProductFamilyModel();

                                apf.ProductFamilyId = res;
                                apf.ProductFamilyName = "Test";
                                apf.ProductDescription = "Test";
                                apfl.Add(apf);
                            }
                            dealerModel.assignedProductFamilyModels = apfl;
                        }
                    }
                }
                dealerModel.DealerId = Guid.NewGuid().ToString();
                var newDealer = await _dealerService.CreateAsync(dealerModel);

                //ShowSuccessMessage("Succssfully Created the Group.");
                _toastNotification.AddSuccessToastMessage("Dealer created Succssfully.");

                //if (newGroup != null)
                //{
                //    return RedirectToAction(nameof(Details), new { id = newGroup.ProductFamilyName });
                //}
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Dealer creation failed: " + ex.Message);
                TempData.Clear();
                _logger.LogError("DealerController-Post: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }



        public void AddSelectedProductFamily([FromBody] DealerModel productfamilyModel)
        {
            if (productfamilyModel != null)
            {
                if (productfamilyModel != null) //&& !string.IsNullOrEmpty(ProductFamilyAssign.DealerId))
                {

                    TempData[CareStreamConst.DealerProductFamily] = productfamilyModel.selectedProductFamily;

                }

            }
        }


        public async Task<IActionResult> Details(string Id)
        {
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var dealer = await _dealerService.GetDealerById(Id);
                    if (dealer != null)
                    {
                        var productFamilyCount = dealer.assignedProductFamilyModels.Count();
                        dealer.productFamilyCount = productFamilyCount;
                        return View(dealer);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Details: Exception occurred...");
                _logger.LogError(ex);
                return View(new DealerModel());
            }

            return View(new DealerModel());

        }


        [HttpPost]
        public async Task<ActionResult> DealerDelete(List<string> selectedItems)
        {
            try
            {
                if (selectedItems != null && selectedItems.Count != 0)
                {

                    if (selectedItems.Any())
                    {
                        //await _dealerService.RemoveDealer(selectedItems);
                        await _dealerService.TempRemoveDealer(selectedItems);

                        _toastNotification.AddSuccessToastMessage("Succssfully deleted the Dealer.");
                        return Ok(GetSuccessMessage("Succssfully deleted the Dealer."));
                    }
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage("Dealer deletion failed: " + ex.Message);
                _logger.LogError("DealerController-DealerDelete: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return Ok();
            //try
            //{
            //    if (selectedItems != null && selectedItems.Count != 0)
            //    {

            //        if (selectedItems.Any())
            //        {
            //            await _dealerService.RemoveDealer(selectedItems);
            //            _toastNotification.AddSuccessToastMessage("Succssfully deleted the Dealer.");
            //            return Ok(GetSuccessMessage("Succssfully deleted the Dealer."));
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    ShowErrorMessage("Dealer deletion failed: " + ex.Message);
            //    _logger.LogError("DealerController-DealerDelete: Exception occurred...");
            //    _logger.LogError(ex);
            //    return RedirectToAction(nameof(Index));
            //}

            //return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteDealerPermanenttly(List<string> selectedItems)
        {
            try
            {
                if (selectedItems != null && selectedItems.Count != 0)
                {

                    if (selectedItems.Any())
                    {
                        await _dealerService.RemoveDealerPermanently(selectedItems);
                        _toastNotification.AddSuccessToastMessage("Succssfully deleted the Dealer.");
                        return Ok(GetSuccessMessage("Succssfully deleted the Dealer."));
                    }
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage("Dealer deletion failed: " + ex.Message);
                _logger.LogError("DealerController-DealerDelete: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return Ok();

        }

        //Upload document
        [HttpPost]
        [ActionName("FileUpload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            //long size = files.Sum(f => f.Length);

            //var filePaths = new List<string>();
            //foreach (var formFile in files)
            //{
            //    if (formFile.Length > 0)
            //    {
            //        // full path to file in temp location
            //        var filePath = Path.GetTempFileName(); //we are using Temp file name just for the example. Add your own file path.
            //        filePaths.Add(filePath);

            //        using (var stream = new FileStream(filePath, FileMode.Create))
            //        {
            //            await formFile.CopyToAsync(stream);
            //        }
            //    }
            //}
            //return Ok(new { count = files.Count, size, filePaths });
            try
            {
                BulkUserFile bulkUserFile = null;
                List<BulkUser> bulkUsers = null;

                ClaimsPrincipal currentUser = this.User;
                //var actionFor = string.IsNullOrEmpty(id) ? CareStreamConst.Bulk_User_Create : id;

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
                            var csvTable = new DataTable();
                            TextReader reader = new StreamReader(filePath);
                            var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                            csv.Read();
                            csv.ReadHeader();
                            var recs=csv.GetRecords<BulkUser>();

                            bulkUserFile.Action = "";
                            bulkUserFile.CreatedDate = DateTime.Now;
                            bulkUserFile.FileName = formFile.FileName;
                            bulkUserFile.FileSize = formFile.Length.ToString();
                            bulkUserFile.Status = CareStreamConst.Bulk_User_Loaded_Status;
                            bulkUserFile.UploadBy = string.IsNullOrEmpty(currentUser.Identity.Name) ? CareStreamConst.Bulk_User_UploadedBy : currentUser.Identity.Name;

                            bulkUsers = ProcessCSVAndCreateDbObject(csv);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"BulkUploadController-Index: Error reading the file name {formFile.FileName} ");
                        _logger.LogError(ex);
                    }

                }

                //    //if (bulkUserFile != null)
                //    //{
                //    //    if (IsBulkUserFileValid(bulkUserFile))
                //    //    {
                //    //        try
                //    //        {
                //    //            _cc.Add(bulkUserFile);
                //    //            var result = _cc.SaveChanges();

                //    //            if (result > 0)
                //    //            {
                //    //                var fileId = bulkUserFile.Id;
                //    //                if (bulkUsers != null)
                //    //                {
                //    //                    if (bulkUsers.Any())
                //    //                    {
                //    //                        foreach (var bulkUser in bulkUsers)
                //    //                        {
                //    //                            try
                //    //                            {
                //    //                                bulkUser.FileId = fileId;
                //    //                                if (IsBulkUserValid(bulkUser, actionFor))
                //    //                                {
                //    //                                    _cc.Add(bulkUser);
                //    //                                    var bulkUserResult = _cc.SaveChanges();

                //    //                                    if (bulkUserResult == 0)
                //    //                                    {
                //    //                                        _logger.LogWarn($"BulkUploadController-Index: Unable to save bulk user for action {bulkUser.Action} and file id {fileId}");
                //    //                                    }
                //    //                                }
                //    //                            }
                //    //                            catch (Exception ex)
                //    //                            {
                //    //                                _logger.LogError($"BulkUploadController-Index: Error saving the user detail for action {bulkUser.Action}");
                //    //                                _logger.LogError(ex);
                //    //                            }
                //    //                        }

                //    //                    }
                //    //                }
                //    //            }
                //    //        }
                //    //        catch (Exception ex)
                //    //        {
                //    //            _logger.LogError($"BulkUploadController-Index: Error saving the file name {bulkUserFile.FileName} ");
                //    //            _logger.LogError(ex);
                //    //        }
                //    //    }
                //    //}

                ShowSuccessMessage("Succssfully uploaded the file.");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("File upload failed: " + ex.Message);
                _logger.LogError("BulkUploadController-Index: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction("Upload");
        }

        public IActionResult Upload()
        {
            //try
            //{
            //    var str = string.IsNullOrEmpty(id) ? string.Empty : id;
            //    TempData[CareStreamConst.Bulk_Action] = str;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("BulkUploadController-Upload: Exception occurred...");
            //    _logger.LogError(ex);
            //}

            return View();
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

        private List<BulkUser> ProcessCSVAndCreateDbObject(CsvReader csvReader)
        {
            List<BulkUser> bulkUsers = new List<BulkUser>();
            try
            {
                if (csvReader != null)
                {
                   
                            #region Create || Update User

                            while (csvReader.Read())
                            {
                                BulkUser bulkUser = new BulkUser
                                {
                                    Action = "",
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
    }

}


