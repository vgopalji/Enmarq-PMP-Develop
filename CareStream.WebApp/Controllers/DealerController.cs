using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Models.Dealer;
using CareStream.Utility;
using CareStream.Utility.DealerService;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
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
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        [Obsolete]
        public DealerController(ILoggerManager logger, IToastNotification toastNotification, IDealerService dealerService, IProductFamilyService productFamilyService, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _toastNotification = toastNotification;
            _dealerService = dealerService;
            _productFamilyService = productFamilyService;
            _hostingEnvironment = hostingEnvironment;
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

        [ActionName("find")]
        public async Task<IActionResult> Index(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    var productFamilyDetails = await _productFamilyService.getProductFamilyById(id);
                    var assignedPF = productFamilyDetails.assignedDealerModels.ToList();
                    List<DealerModel> dealrList = new List<DealerModel>();
                    foreach (var res in assignedPF)
                    {
                        var dealerDetails = await _dealerService.GetDealerById(res.DealerId);
                        dealrList.Add(dealerDetails);
                    }

                    if (dealrList != null)
                    {
                        return View("Index", dealrList);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DealerController-Index: Exception occurred .....");
                _logger.LogError(ex);
            }

            return View(new DealerModel());
        }
        public async Task<IActionResult> Create()
        {
            DealerModel dm = new DealerModel();
            TempData.Clear();
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
                            List<string> productFamilyList = new List<string>(selectedProductFamily);
                            foreach (var productFamilyId in productFamilyList)
                            {
                                AssignedProductFamilyModel apf = new AssignedProductFamilyModel();
                                var productFamily = await _productFamilyService.getProductFamilyById(productFamilyId);

                                apf.ProductFamilyId = productFamily.ProductFamilyId;
                                apf.ProductFamilyName = productFamily.ProductFamilyName;
                                apf.ProductDescription = productFamily.ProductDescription;
                                apfl.Add(apf);
                            }
                            dealerModel.assignedProductFamilyModels = apfl;
                        }
                    }
                }
                dealerModel.DealerId = Guid.NewGuid().ToString();
                var newDealer = await _dealerService.CreateAsync(dealerModel);

                ShowSuccessMessage("Succssfully Created the Group.");
                //_toastNotification.AddSuccessToastMessage("Dealer created Succssfully.");

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
                        ShowSuccessMessage("Succssfully deleted the dealer.");
                        //_toastNotification.AddSuccessToastMessage("Succssfully deleted the Dealer.");
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
                        //_toastNotification.AddSuccessToastMessage("Succssfully deleted the Dealer.");
                        ShowSuccessMessage("Succssfully deleted Dealer.");
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
            try
            {
                FileDetails fileDetails = null;
                List<BulkUpload> bulkUsers = new List<BulkUpload>();
                ClaimsPrincipal currentUser = this.User;
                //var actionFor = string.IsNullOrEmpty(id) ? CareStreamConst.Bulk_User_Create : id;
                var count = 0;
                var recCount = 0;
                var successFile = 0;
                var failureFiles = 0;
                foreach (var formFile in files)
                {
                    try
                    {
                        if (formFile.Length > 0)
                        {
                            fileDetails = new FileDetails();

                            var filePath = Path.GetTempFileName();

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            TextReader reader = new StreamReader(filePath);
                            var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                            csv.Read();
                            csv.ReadHeader();
                            var resultsDealer = csv.GetRecords<BulkUpload>();
                            List<AssignedProductFamilyModel> apfml = new List<AssignedProductFamilyModel>();

                            foreach (var res in resultsDealer)
                            {
                                if (res != null)
                                {
                                    //BulkUpload blu = new BulkUpload();
                                    DealerModel dmUpload = new DealerModel();
                                    dmUpload.DealerName = res.DealerName;
                                    dmUpload.DealerDescription = res.DealerDescription;
                                    dmUpload.SAPID = res.SAPID;
                                    dmUpload.DealerId = Guid.NewGuid().ToString();
                                    var productFamily = await _productFamilyService.GetProductFamilyByName(res.ProductFamilyName);
                                    if (productFamily != null)
                                    {
                                        AssignedProductFamilyModel apfm = new AssignedProductFamilyModel();                                   
                                        apfm.ProductDescription = productFamily.ProductFamilyName;
                                        apfm.ProductFamilyName = productFamily.ProductDescription;
                                        apfm.ProductFamilyId = productFamily.ProductFamilyId;
                                        apfml.Add(apfm);
                                    }
                                    dmUpload.assignedProductFamilyModels = apfml;
                                    //blu.DealerName = res.DealerName;
                                    //blu.DealerDescription = res.DealerDescription;
                                    //blu.SAPID = res.SAPID;
                                    //bulkUsers.Add(blu);
                                    try
                                    {
                                        await _dealerService.CreateAsync(dmUpload);
                                        successFile++;
                                    }
                                    catch
                                    {
                                        failureFiles++;
                                    }
                                }
                            }
                            //fileDetails.Action = "";
                            fileDetails.FileDetailsId = Guid.NewGuid().ToString();
                            fileDetails.CreatedDate = DateTime.Now;
                            fileDetails.FileName = formFile.FileName;
                            fileDetails.Success = successFile.ToString();
                            fileDetails.Failure = failureFiles.ToString();
                            fileDetails.Status = "complted with no error";//CareStreamConst.Bulk_User_Loaded_Status;
                            fileDetails.UploadBy = string.IsNullOrEmpty(currentUser.Identity.Name) ? CareStreamConst.Bulk_User_UploadedBy : currentUser.Identity.Name;

                            if (fileDetails != null)
                            {
                                await _dealerService.AddFileDetails(fileDetails);
                            }
                            count++;

                            //bulkUsers = ProcessCSVAndCreateDbObject(csv);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"BulkUploadController-Index: Error reading the file name {formFile.FileName} ");
                        _logger.LogError(ex);
                    }

                }

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

        private List<BulkUpload> ProcessCSVAndCreateDbObject(CsvReader csvReader)
        {
            List<BulkUpload> bulkUsers = new List<BulkUpload>();
            try
            {
                if (csvReader != null)
                {

                    #region Create || Update User

                    while (csvReader.Read())
                    {
                        BulkUpload bulkUser = new BulkUpload
                        {

                            //Status = CareStreamConst.Bulk_User_Loaded_Status,
                            //CreatedDate = DateTime.UtcNow,
                            //ModifiedDate = DateTime.UtcNow
                        };

                        foreach (string header in csvReader.Context.HeaderRecord)
                        {
                            try
                            {
                                switch (header.ToLower())
                                {
                                    case "DealerName":
                                        bulkUser.DealerName = csvReader.GetField(header);
                                        break;
                                    case "DealerDescription":
                                        bulkUser.DealerDescription = csvReader.GetField(header);
                                        break;
                                    case "SAPID":
                                        bulkUser.SAPID = csvReader.GetField(header);
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

        [ActionName("FileUploadResult")]
        public async Task<ActionResult> FileUploadResult()
        {
            try
            {
                var DealerUploadResult = await _dealerService.GetFileDetails();
                return View(DealerUploadResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("FileUploadResult", new List<DealerModel>());
            }
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

        private string GetTemplateFileName(string action)
        {
            string fileName;
            switch (action)
            {
                case CareStreamConst.Bulk_Dealer_Create:
                    fileName = "DealerCreateTemplate.csv";
                    break;
                case CareStreamConst.Bulk_Dealer_Invite:
                    fileName = "DealerInviteTemplate.csv";
                    break;
                case CareStreamConst.Bulk_Dealer_Update:
                    fileName = "DealerUpdateTemplate.csv";
                    break;
                case CareStreamConst.Bulk_Dealer_Delete:
                    fileName = "DealerDeleteTemplate.csv";
                    break;
                default:
                    fileName = "Template.csv";
                    break;
            }
            return fileName;
        }
        [HttpPost]
        public async Task<ActionResult> RestoreDealer([FromBody] DeletedDealerModel deletedDealerModel)
        {
            try
            {
                //DealerModel dmcList = new DealerModel();
                List<AssignedProductFamilyModel> apfList = new List<AssignedProductFamilyModel>();
                //List<DealerModel> dmList = new List<DealerModel>();
                if (deletedDealerModel.selectedProductFamily != null)
                {
                    List<string> dealerList = new List<string>(deletedDealerModel.selectedProductFamily);
                    if (dealerList != null)
                    {
                        DealerModel dm = new DealerModel();
                        foreach (var dealerId in dealerList)
                        {
                            var dealers = await _dealerService.GetDeletedDealersById(dealerId);

                            dm.DealerId = dealers.DealerId;
                            dm.DealerName = dealers.DealerName;
                            dm.DealerDescription = dealers.DealerDescription;
                            dm.SAPID = dealers.SAPID;
                            foreach (var res in dealers.deletedDealerProductFamilyModels)
                            {
                                AssignedProductFamilyModel apm = new AssignedProductFamilyModel();
                                apm.ProductFamilyId = res.ProductFamilyId;
                                apm.ProductFamilyName = res.ProductFamilyName;
                                apm.ProductDescription = res.ProductDescription;
                                apfList.Add(apm);
                                dm.assignedProductFamilyModels = apfList;
                            }

                            var newDealer = await _dealerService.CreateAsync(dm);
                            await _dealerService.RemoveDealerInRestoreById(dm.DealerId);
                        }
                        ShowSuccessMessage("Succssfully Restored Dealer.");

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Restore: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }


}






