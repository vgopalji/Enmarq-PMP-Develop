using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Models.Dealer;
using CareStream.Models.ProductFamily;
using CareStream.Utility;
using CareStream.Utility.DealerService;
using CareStream.Utility.Services;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos.Linq;
using NToastNotify;
using NToastNotify.Helpers;

namespace CareStream.WebApp.Controllers
{
    public class ProductFamilyController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IProductFamilyService _productFamilyService;
        private readonly IToastNotification _toastNotification;
        private readonly IDealerService _dealerService;
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        [Obsolete]
        public ProductFamilyController(IProductFamilyService productFamilyService, ILoggerManager logger, IToastNotification toastNotification, IDealerService dealerService, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _productFamilyService = productFamilyService;
            _toastNotification = toastNotification;
            _dealerService = dealerService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {

            try
            {
                var productFamily = await _productFamilyService.GetAsync();
                return View(productFamily);
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("Index", new List<ProductFamilyModel>());
            }
        }

        public async Task<IActionResult> Details(string Id)
        {
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var productFamily = await _productFamilyService.getProductFamilyById(Id);
                    if (productFamily != null)
                    {
                        var dealerCount = productFamily.assignedDealerModels.Count();
                        productFamily.dealerCount = dealerCount;
                        return View(productFamily);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyController-Details: Exception occurred...");
                _logger.LogError(ex);
                return View(new ProductFamilyModel());
            }

            return View(new ProductFamilyModel());

        }
        [ActionName("find")]
        public async Task<IActionResult> Index(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    var dealerDetails = await _dealerService.GetDealerById(id);
                    var assignedPF = dealerDetails.assignedProductFamilyModels.ToList();
                    List<ProductFamilyModel> productFamilyList = new List<ProductFamilyModel>();
                    foreach (var res in assignedPF)
                    {
                        var productFamilyDetails = await _productFamilyService.getProductFamilyById(res.ProductFamilyId);
                        productFamilyList.Add(productFamilyDetails);
                    }

                    if (productFamilyList != null)
                    {
                        return View("Index", productFamilyList);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ProductFamilyController-Index: Exception occurred .....");
                _logger.LogError(ex);
            }

            return View(new ProductFamilyModel());
        }

        public async Task<IActionResult> Create()
        {
            ProductFamilyModel pf = new ProductFamilyModel();
            TempData.Clear();
            var productFamily = await _dealerService.GetAllDealers();
            try
            {
                pf.dealerModel = productFamily;
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerController-Create: Exception occurred...");
                _logger.LogError(ex);
                return View("create", new ProductFamilyModel());
            }
            return View(pf);

        }

        public async Task<IActionResult> Post([FromForm] ProductFamilyModel productFamilyModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", productFamilyModel);
                }

                if (productFamilyModel != null)
                {
                    if (string.IsNullOrEmpty(productFamilyModel.ProductDescription) || string.IsNullOrEmpty(productFamilyModel.ProductFamilyName))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                DealerModel dmcList = new DealerModel();
                List<AssignedDealerModel> apfl = new List<AssignedDealerModel>();

                if (TempData[CareStreamConst.ProductFamilySelectedDealer] != null)
                {
                    if (TempData[CareStreamConst.ProductFamilySelectedDealer] is string[])
                    {
                        var selectedProductFamily = TempData[CareStreamConst.ProductFamilySelectedDealer] as string[];
                        if (selectedProductFamily != null)
                        {
                            List<string> list = new List<string>(selectedProductFamily);
                            foreach (var res in list)
                            {
                                AssignedDealerModel adm = new AssignedDealerModel();
                                var dealers = await _dealerService.GetDealerById(res);
                                adm.DealerName = dealers.DealerName;
                                adm.DealerDescription = dealers.DealerDescription;
                                adm.DealerId = dealers.DealerId;
                                apfl.Add(adm);
                            }
                            productFamilyModel.assignedDealerModels = apfl;
                        }
                    }
                }
                productFamilyModel.ProductFamilyId = Guid.NewGuid().ToString();
                var newDealer = await _productFamilyService.CreateAsync(productFamilyModel);

                ShowSuccessMessage("Succssfully Created the product family.");
                //_toastNotification.AddSuccessToastMessage("Dealer created Succssfully.");

                //if (newGroup != null)
                //{
                //    return RedirectToAction(nameof(Details), new { id = newGroup.ProductFamilyName });
                //}
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Product Family creation failed: " + ex.Message);
                TempData.Clear();
                _logger.LogError("ProductFamilyController-Post: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));

        }

        public void AddSelectedDealer([FromBody] ProductFamilyModel productfamilyModel)
        {
            if (productfamilyModel != null)
            {
                if (productfamilyModel != null) //&& !string.IsNullOrEmpty(ProductFamilyAssign.DealerId))
                {

                    TempData[CareStreamConst.ProductFamilySelectedDealer] = productfamilyModel.selectedDealer;

                }

            }
        }

        [HttpPost]
        public async Task<ActionResult> ProductFamilyDelete(List<string> selectedItems)
        {
            try
            {
                if (selectedItems != null && selectedItems.Count != 0)
                {
                    if (selectedItems.Any())
                    {
                        await _productFamilyService.TempRemoveProductFamily(selectedItems);
                        ShowSuccessMessage("Succssfully deleted the product family.");
                        //_toastNotification.AddSuccessToastMessage("Succssfully deleted the Dealer.");
                        return Ok(GetSuccessMessage("Succssfully deleted the product family."));
                    }
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage("Product Family deletion failed: " + ex.Message);
                _logger.LogError("ProductFamilyController-ProductFamilyDelete: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }
            return Ok();

        }
        [HttpPost]
        public async Task<ActionResult> DeleteProductFamilyPermanenttly(List<string> selectedItems)
        {
            try
            {
                if (selectedItems != null && selectedItems.Count != 0)
                {
                    if (selectedItems.Any())
                    {
                        await _productFamilyService.RemoveProductFamilyPermanently(selectedItems);
                        //_toastNotification.AddSuccessToastMessage("Succssfully deleted the Product Family.");
                        ShowSuccessMessage("Succssfully deleted the product family.");
                        return Ok(GetSuccessMessage("Succssfully deleted the Product Family."));
                    }
                }

            }
            catch (Exception ex)
            {
                ShowErrorMessage("ProductFamily deletion failed: " + ex.Message);
                _logger.LogError("ProductFamilyController-ProductFamilyDelete: Exception occurred...");
                _logger.LogError(ex);
                return RedirectToAction(nameof(Index));
            }

            return Ok();
        }

        public async Task<IActionResult> Deleted()
        {
            try
            {
                var deletedProductFamily = await _productFamilyService.GetDeletedProductFamilies();
                return View(deletedProductFamily);
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("Deleted", new List<DeletedProductFamily>());
            }


        }
        public IActionResult Upload()
        {
            try
            {
                return View();

            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyController-Upload: Exception occurred...");
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
                _logger.LogError("ProductFamilyController-Download: Exception occurred...");
                _logger.LogError(ex);
                return null;
            }
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
                            var productFamilyRecords = csv.GetRecords<ProductFamilyBulkUpload>();
                            List<AssignedDealerModel> apfml = new List<AssignedDealerModel>();
                            foreach (var rec in productFamilyRecords)
                            {
                                if (rec != null)
                                {                                   
                                    ProductFamilyModel pfUpload = new ProductFamilyModel();
                                    pfUpload.ProductFamilyName = rec.ProductFamilyName;
                                    pfUpload.ProductDescription = rec.ProductDescription;
                                    pfUpload.ProductFamilyId = Guid.NewGuid().ToString();
                                    var dealerDetails = await _dealerService.GetDealeryByName(rec.DealerName);
                                    if (dealerDetails != null)
                                    {
                                        AssignedDealerModel dealerAssign = new AssignedDealerModel();
                                        dealerAssign.DealerName = dealerDetails.DealerName;
                                        dealerAssign.DealerDescription = dealerDetails.DealerDescription;
                                        dealerAssign.DealerId = dealerDetails.DealerId;
                                        apfml.Add(dealerAssign);
                                    }
                                    pfUpload.assignedDealerModels = apfml;                                   
                                    try
                                    {
                                        await _productFamilyService.CreateAsync(pfUpload);
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
                _logger.LogError("ProductFamilyController-Index: Exception occurred...");
                _logger.LogError(ex);
            }

            return RedirectToAction("Upload");
        }

        [ActionName("FileUploadResult")]
        public async Task<ActionResult> FileUploadResult()
        {
            try
            {
                var ProductFamilyUploadResult = await _productFamilyService.GetFileDetailsByFileName();
                return View(ProductFamilyUploadResult);
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyController-Index: Exception occurred...");
                _logger.LogError(ex);
                return View("FileUploadResult", new List<ProductFamilyModel>());
            }
        }
        [HttpPost]
        public async Task<ActionResult> RestoreProductFamily([FromBody] DeletedProductFamily deletedProductFamily)
        {
            try
            {
                //DealerModel dmcList = new DealerModel();
                List<AssignedDealerModel> ADMList = new List<AssignedDealerModel>();
                //List<DealerModel> dmList = new List<DealerModel>();
                if (deletedProductFamily.selectedDealer != null)
                {
                    List<string> productFamilyList = new List<string>(deletedProductFamily.selectedDealer);
                    if (productFamilyList != null)
                    {
                        ProductFamilyModel productFamily = new ProductFamilyModel();
                        foreach (var pf in productFamilyList)
                        {
                            var deletedPF = await _productFamilyService.GetDeletedProductFamilyById(pf);

                            productFamily.ProductFamilyId = deletedPF.ProductFamilyId;
                            productFamily.ProductFamilyName = deletedPF.ProductFamilyName;
                            productFamily.ProductDescription = deletedPF.ProductDescription;                            
                            foreach (var res in deletedPF.deletedProductFamilyDealerModels)
                            {
                                AssignedDealerModel ASM = new AssignedDealerModel();
                                ASM.DealerId = res.DealerId;
                                ASM.DealerName = res.DealerName;
                                ASM.DealerDescription = res.DealerDescription;
                                ADMList.Add(ASM);
                                productFamily.assignedDealerModels = ADMList;
                            }

                            var restorePF = await _productFamilyService.CreateAsync(productFamily);
                            await _productFamilyService.RemovePFinRestoreById(productFamily.ProductFamilyId);
                        }
                        ShowSuccessMessage("Succssfully Restored ProductFamily.");

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
        #region PrivateMethods
        private string GetTemplateFileName(string action)
        {
            string fileName;
            switch (action)
            {
                case CareStreamConst.Bulk_ProductFamily_Create:
                    fileName = "ProductFamilyCreateTemplate.csv";
                    break;
                case CareStreamConst.Bulk_Dealer_Invite:
                    fileName = "ProductFamilyInviteTemplate.csv";
                    break;
                case CareStreamConst.Bulk_Dealer_Update:
                    fileName = "ProductFamilyUpdateTemplate.csv";
                    break;
                case CareStreamConst.Bulk_Dealer_Delete:
                    fileName = "ProductFamilyDeleteTemplate.csv";
                    break;
                default:
                    fileName = "Template.csv";
                    break;
            }
            return fileName;
        }
        #endregion
    }
}