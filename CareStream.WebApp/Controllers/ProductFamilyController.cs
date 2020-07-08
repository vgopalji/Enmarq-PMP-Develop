using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility;
using CareStream.Utility.DealerService;
using CareStream.Utility.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NToastNotify;

namespace CareStream.WebApp.Controllers
{
    public class ProductFamilyController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IProductFamilyService _productFamilyService;
        private readonly IToastNotification _toastNotification;
        private readonly IDealerService _dealerService;

        public ProductFamilyController(IProductFamilyService productFamilyService, ILoggerManager logger, IToastNotification toastNotification, IDealerService dealerService)
        {
            _logger = logger;
            _productFamilyService = productFamilyService;
            _toastNotification = toastNotification;
            _dealerService = dealerService;
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
                    _logger.LogError("DealerController-Index: Exception occurred...");
                    _logger.LogError(ex);
                    return View("Index", new List<ProductFamilyModel>());
                }
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
                    List<ProductFamilyModel> pfml = new List<ProductFamilyModel>();
                    foreach(var res in assignedPF)
                    {
                        var productFamilyDetails =await _productFamilyService.getProductFamilyById(res.ProductFamilyId);
                        pfml.Add(productFamilyDetails);
                    }

                    if (pfml != null)
                    {
                        return View("Index",pfml);
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

        //public async Task<IActionResult> Create()
        //{
        //    //var productFamilyModelCosmos = new ProductFamilyCosmos();
        //    ProductFamilyViewModel pfvmCosmos = new ProductFamilyViewModel();

        //    pfvmCosmos.DealerModelCosmos = _dealerService.GetDealerDetails();

        //    try
        //    {
        //        //TempData.Clear();
        //        //var dealerResult = await _productFamilyService.GetAsync();

        //        //if (dealerResult != null)
        //        //{
        //        //    if (dealerResult.Any())
        //        //    {
        //        //        foreach (var dealerRec in dealerResult)
        //        //        {
        //        //            foreach(var res in dealerRec.dealerModelCosmos)
        //        //            {
        //        //                try
        //        //                {
        //        //                    DealerModelCosmos dmc = new DealerModelCosmos();
        //        //                    dmc.DealerId = res.DealerId;
        //        //                    dmc.DealerName = res.DealerName;
        //        //                    dmc.DealerDescription = res.DealerDescription;
        //        //                    dmc.SAPID = res.SAPID;
        //        //                    //productFamilyModelCosmos.dealerModelCosmos = dealerRec;
        //        //                    productFamilyModelCosmos.dealerModelCosmos.Add(dmc);

        //        //                }
        //        //                catch (Exception ex)
        //        //                {
        //        //                    _logger.LogError($"ProductFamilyController-Create: Exception occurred while adding the Dealer Assigment for {res.DealerName}");
        //        //                    _logger.LogError(ex);
        //        //                }
        //        //            }

        //        //        }

        //        //    }
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("GroupController-Create: Exception occurred...");
        //        _logger.LogError(ex);
        //        return View(pfvmCosmos);
        //    }

        //    return View(pfvmCosmos);
        //}
        public async Task<IActionResult> Create()
        {
            //    DealerModel dm = new DealerModel();
            //    var productFamily = await _productFamilyService.getAllProductFamily();
            //    try
            //    {
            //        dm.productFamilyModels = productFamily;
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError("DealerController-Create: Exception occurred...");
            //        _logger.LogError(ex);
            //        return View("create", new DealerModel());
            //    }
            //    return View(dm);
            return View("create", new ProductFamilyModel());
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

                //DealerModel dmcList = new DealerModel();
                //if (TempData[CareStreamConst.DealerProductFamily] != null)
                //{
                //    if (TempData[CareStreamConst.Dealer] is string[])
                //    {
                //        var selectedProductFamily = TempData[CareStreamConst.DealerProductFamily] as string[];
                //        if (selectedProductFamily != null)
                //        {
                //            List<string> list = new List<string>(selectedProductFamily);
                //            foreach (var res in list)
                //            {
                //                ProductFamilyModel dmc = new ProductFamilyModel();
                //                dmc.ProductFamilyId = res;
                //                dmcList.productFamilyModels.Add(dmc);
                //            }
                //        }
                //    }
                //}
                productFamilyModel.ProductFamilyId = Guid.NewGuid().ToString();
                var newDealer = await _productFamilyService.CreateAsync(productFamilyModel);

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

        //public async Task<IActionResult> Post([FromForm] ProductFamilyCosmos productFamilyCosmos)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            //BindViewDataForGroup(groupModel);
        //            return View("Create", productFamilyCosmos);
        //        }

        //        if (productFamilyCosmos != null)
        //        {
        //            if (string.IsNullOrEmpty(productFamilyCosmos.ProductDescription) || string.IsNullOrEmpty(productFamilyCosmos.ProductFamilyName))
        //            {
        //                return RedirectToAction(nameof(Index));
        //            }
        //        }

        //        //List<DealerModelCosmos> dmcList = new List<DealerModelCosmos>(); 

        //        //if (TempData[CareStreamConst.Dealer] != null)
        //        //{
        //        //    if (TempData[CareStreamConst.Dealer] is string[])
        //        //    {
        //        //        var selectDealer = TempData[CareStreamConst.Dealer] as string[];
        //        //        if (selectDealer != null)
        //        //        {
        //        //            List<string> list = new List<string>(selectDealer);

        //        //            //List<DealerModelCosmos> objectList = list.Cast<DealerModelCosmos>().ToList();
        //        //            foreach(var res in list)
        //        //            {
        //        //                DealerModelCosmos dmc = new DealerModelCosmos();
        //        //                dmc.DealerId = Guid.NewGuid().ToString();
        //        //                dmc.DealerName = res;
        //        //                dmc.DealerDescription = res;
        //        //                dmc.SAPID = "j3jj343kjl3j43jlkj43j";
        //        //                //dmcList.Add(dmc);
        //        //               var ret=await _dealerService.CreateAsync(dmc);

        //        //            }
        //        //        }
        //        //    }
        //        //}
        //        //var insertDealer = _dealerService.CreateAsync(dmcList);
        //        productFamilyCosmos.ProductFamilyId = Guid.NewGuid().ToString();
        //        var newDealer = await _productFamilyService.CreateAsync(productFamilyCosmos);

        //        //ShowSuccessMessage("Succssfully Created the Group.");
        //        _toastNotification.AddSuccessToastMessage("Succssfully Created the Product Family.");

        //        //if (newGroup != null)
        //        //{
        //        //    return RedirectToAction(nameof(Details), new { id = newGroup.ProductFamilyName });
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowErrorMessage("ProductFamily creation failed: " + ex.Message);
        //        TempData.Clear();
        //        _logger.LogError("ProductFamilyController-Post: Exception occurred...");
        //        _logger.LogError(ex);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return RedirectToAction(nameof(Index));
        //}



        //[HttpPost]
        //public async Task<ActionResult> GroupDelete(List<string> selectedItems)
        //{
        //    try
        //    {
        //        if (selectedItems != null && selectedItems.Count != 0)
        //        {

        //            if (selectedItems.Any())
        //            {
        //                await _productFamilyService.RemoveGroup(selectedItems);

        //                return Ok(GetSuccessMessage("Succssfully deleted the Group."));
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ShowErrorMessage("Group deletion failed: " + ex.Message);
        //        _logger.LogError("GroupController-GroupDelete: Exception occurred...");
        //        _logger.LogError(ex);
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return Ok();
        //}
    }
}