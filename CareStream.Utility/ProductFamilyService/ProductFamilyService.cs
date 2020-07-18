﻿using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Models.Dealer;
using CareStream.Utility.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Directory = System.IO.Directory;

namespace CareStream.Utility
{
    //public interface IProductFamilyService: ICosmosDbService<ProductFamilyModel>
    public interface IProductFamilyService : ICosmosDbService<ProductFamilyModel>

    {
       
        Task<List<ProductFamilyModel>> getAllProductFamily();
        Task<ProductFamilyModel> getProductFamilyById(string Id);
        Task<ProductFamilyModel> GetProductFamilyByName(string Name);
        Task TempRemoveProductFamily(List<string> ProductFamilyIdsToDelete);
        Task RemoveProductFamilyPermanently(List<string> productFamilysToDelete);
        Task<List<DeletedProductFamily>> GetDeletedProductFamilies();
        Task<List<FileDetails>> GetFileDetailsByFileName();
        public Task<DeletedProductFamily> GetDeletedProductFamilyById(string Id);
        Task<bool> RemovePFinRestoreById(string Id);
    }

    //public class ProductFamilyService : CosmosDBService<ProductFamilyModel>, IProductFamilyService
    public class ProductFamilyService : CosmosDBService<ProductFamilyModel>, IProductFamilyService
    {
        private readonly ILoggerManager _logger;
        //private IGroupOwnerService _groupOwnerService;
        //private IGroupMemberService _groupMemberService;
        private readonly CosmosDbContext _cosmosDbContext;


        public ProductFamilyService(ILoggerManager logger, CosmosDbContext cosmosDbContext):base(cosmosDbContext)
        {
            _logger = logger;
            _cosmosDbContext = cosmosDbContext;
            //_groupOwnerService = groupOwnerService;
            //_groupMemberService = groupMemberService; 
        }

        public ProductFamilyService(CosmosDbContext cosmosDbContext) :base(cosmosDbContext) 
        {
            _logger = new LoggerManager();
        }



        //#region MVC Controller Helpers
        //public async Task<List<DealerAssignModel>> BuildProductDealer()
        //{
        //    var dealerAssignModel = new List<DealerAssignModel>();
        //    try
        //    {
        //        _logger.LogInfo("ProductFamilyService-BuildProductDealer: [Started] to get detail list of product family to build dealer");
        //        var dealers = GetAllDealerDetails();
        //        if (dealers != null)
        //        {
        //            DealerAssignModel dealerAssignModelLocal = new DealerAssignModel();
        //            foreach (var aDict in dealers.OrderBy(x => x.Value))
        //            {
        //                dealerAssignModelLocal.DealerList.Add(aDict.Key, aDict.Value);
        //            }
        //            dealerAssignModel.Add(dealerAssignModelLocal);

        //        }
        //        _logger.LogInfo("ProductFamilyService-BuildProductDealer: [Completed] to getting detail list of user to build Dealer");

        //    }
        //    catch (ServiceException ex)
        //    {
        //        _logger.LogError("ProductFamilyService-BuildProductDealer: Exception occured....");
        //        _logger.LogError(ex);
        //    }
        //    return dealerAssignModel;
        //}

        //public async Task<List<ProductFamilyModel>> GetDetailProductFamilyList()
        //{
        //    try
        //    {
        //        List<ProductFamilyModel> productFamilyList = new List<ProductFamilyModel>();

        //        //var options = new JsonSerializerOptions
        //        //{
        //        //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        //        //    WriteIndented = true
        //        //};

        //        //var jsonString = System.IO.File.ReadAllText("D:\\New folder\\CareStream.Utility\\App_Data\\ProductFamily.json");                   
        //        // var jsonModel = System.Text.Json.JsonSerializer.Deserialize<ProductFamily>(jsonString, options);
        //        //var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"carestream.Utility\\{"App_Data\\ProductFamily.json"}");

        //        int count = 0;
        //        string jsonString = System.IO.File.ReadAllText("D:\\New folder\\CareStream.Utility\\App_Data\\ProductFamily.json");

        //        using (JsonDocument document = JsonDocument.Parse(jsonString))
        //        {
        //            JsonElement root = document.RootElement;
        //            JsonElement productFamilyElement = root.GetProperty("ProductFamily");

        //            count = productFamilyElement.GetArrayLength();

        //            foreach (JsonElement productFamily in productFamilyElement.EnumerateArray())
        //            {
        //                if (productFamily.TryGetProperty("productFamilyName", out JsonElement productFamilyName) && productFamily.TryGetProperty("productDescription", out JsonElement productFamilyDescription))
        //                {
        //                    ProductFamilyModel pf = new ProductFamilyModel();
        //                    //sum += gradeElement.GetDouble();
        //                    pf.ProductFamilyName = productFamilyName.GetString();
        //                    pf.ProductDescription = productFamilyDescription.GetString();
        //                    productFamilyList.Add(pf);

        //                }

        //                else
        //                {
        //                    return null;
        //                }
        //            }

        //        }
        //        if (productFamilyList == null)
        //        {
        //            return null;
        //        }
        //        return productFamilyList;
        //    }
        //    catch (ServiceException ex)
        //    {
        //        _logger.LogError("ProductFamily-GetDetailProductFamilyList: Exception occured....");
        //        _logger.LogError(ex);
        //        throw ex;
        //    }


        //}

        //public List<DealerModel> GetDealerDetails()
        //{
        //    try
        //    {
        //        List<DealerModel> dealerList = new List<DealerModel>();
        //        int count = 0;
        //        string jsonString = System.IO.File.ReadAllText("D:\\New folder\\CareStream.Utility\\App_Data\\Dealer.json");

        //        using (JsonDocument document = JsonDocument.Parse(jsonString))
        //        {
        //            JsonElement root = document.RootElement;
        //            JsonElement productFamilyElement = root.GetProperty("Dealers");

        //            count = productFamilyElement.GetArrayLength();

        //            foreach (JsonElement productFamily in productFamilyElement.EnumerateArray())
        //            {
        //                if (productFamily.TryGetProperty("dealerId", out JsonElement dealerId) && productFamily.TryGetProperty("dealerName", out JsonElement dealerName) && productFamily.TryGetProperty("dealerDescription", out JsonElement dealerDescription) && productFamily.TryGetProperty("SAPID", out JsonElement SAPID))
        //                {
        //                    DealerModel dealer = new DealerModel();
        //                    //sum += gradeElement.GetDouble();
        //                    dealer.DealerId = dealerId.GetString();
        //                    dealer.DealerName = dealerName.GetString();
        //                    dealer.DealerDescription = dealerDescription.GetString();
        //                    dealer.SAPID = SAPID.GetString();
        //                    dealerList.Add(dealer);
        //                }

        //                else
        //                {
        //                    return null;
        //                }
        //            }
        //        }
        //        if (dealerList == null)
        //        {
        //            return null;
        //        }
        //        return dealerList;
        //    }
        //    catch (ServiceException ex)
        //    {
        //        _logger.LogError("ProductFamily-GetDetailProductFamilyList: Exception occured....");
        //        _logger.LogError(ex);
        //        throw ex;
        //    }

        //}

        //public async Task<List<ProductFamilyModel>> CreateProductFamily(ProductFamilyModel productFamilyModel)
        //{
        //    List<ProductFamilyModel> list;
        //    //var list = JsonConvert.DeserializeObject<List<ProductFamily1>>(productFamily1.);
        //    list =await GetDetailProductFamilyList();
        //    ProductFamilyModel productList = new ProductFamilyModel();
        //    productList.ProductFamilyName = productFamilyModel.ProductFamilyName;
        //    productList.ProductDescription = productFamilyModel.ProductDescription;
        //    list.Add(productList);
        //    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
        //    var productFamily =await GetDetailProductFamilyList();
        //    return productFamily;

        //}
        //public async Task RemoveGroup(List<string> groupIdsToDelete)
        //{
        //    try
        //    {
        //        if (groupIdsToDelete == null)
        //        {
        //            _logger.LogError("GroupService-RemoveGroup: Input value cannot be empty");
        //            return;
        //        }


        //        //GraphServiceClient client = GraphClientUtility.GetGraphServiceClient();
        //        //if (client == null)
        //        //{
        //        //    _logger.LogError("GroupService-RemoveGroup: Unable to create object for graph client");
        //        //    return;
        //        //}

        //        foreach (var id in groupIdsToDelete)
        //        {
        //            try
        //            {

        //                _logger.LogInfo($"GroupService-RemoveGroup: [Started] removing group for id [{id}] on Azure AD B2C");
        //                var res=await _cosmosDbContext.productFamilyCosmos.FindAsync(id);
        //                //await client.Groups[id].Request().DeleteAsync();
        //                var ress= _cosmosDbContext.productFamilyCosmos.Remove(res);
        //                _cosmosDbContext.SaveChanges();
        //                _logger.LogInfo($"GroupService-RemoveGroup: [Completed] removed group [{id}] on Azure AD B2C");
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError($"GroupService-RemoveGroup: Exception occured while removing group for id [{id}]");
        //                _logger.LogError(ex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("GroupService-RemoveGroup: Exception occured....");
        //        _logger.LogError(ex);
        //        throw ex;
        //    }
        //}

        //#endregion

        //#region Private Methods
        //private Dictionary<string, string> GetAllDealerDetails()
        //{
        //    Dictionary<string, string> dealerList = null;
        //    try
        //    {
        //        dealerList = new Dictionary<string, string>();
        //        int count = 0;
        //        string jsonString = System.IO.File.ReadAllText("D:\\New folder\\CareStream.Utility\\App_Data\\Dealer.json");

        //        using (JsonDocument document = JsonDocument.Parse(jsonString))
        //        {
        //            JsonElement root = document.RootElement;
        //            JsonElement productFamilyElement = root.GetProperty("Dealers");

        //            count = productFamilyElement.GetArrayLength();

        //            foreach (JsonElement productFamily in productFamilyElement.EnumerateArray())
        //            {
        //                if (productFamily.TryGetProperty("dealerId", out JsonElement dealerId) && productFamily.TryGetProperty("dealerName", out JsonElement dealerName) && productFamily.TryGetProperty("dealerDescription", out JsonElement dealerDescription) && productFamily.TryGetProperty("SAPID", out JsonElement SAPID))
        //                {
        //                    DealerModel dealer = new DealerModel();
        //                    //sum += gradeElement.GetDouble();
        //                    dealer.DealerId = dealerId.GetString();
        //                    dealer.DealerName = dealerName.GetString();
        //                    dealer.DealerDescription = dealerDescription.GetString();
        //                    dealer.SAPID = SAPID.GetString();
        //                    dealerList.Add(Convert.ToString(dealer.DealerId), dealer.DealerName);

        //                }

        //                else
        //                {
        //                    return null;
        //                }
        //            }

        //        }
        //        if (dealerList == null)
        //        {
        //            return null;
        //        }
        //        return dealerList;
        //    }
        //    catch (ServiceException ex)
        //    {
        //        _logger.LogError("ProductFamily-GetDetailProductFamilyList: Exception occured....");
        //        _logger.LogError(ex);
        //        throw ex;
        //    }

        //}
        //#endregion

        public Task<List<ProductFamilyModel>> getAllProductFamily()
        {
            var productFamily = _cosmosDbContext.productFamily.ToListAsync();
            return productFamily;
        }
        public Task<ProductFamilyModel> getProductFamilyById(string Id)
        {
            var productFamily = _cosmosDbContext.productFamily.Where(d => d.ProductFamilyId == Id).SingleOrDefaultAsync();
            return productFamily;
        }
        public async Task<ProductFamilyModel> GetProductFamilyByName(string Name)
        {
            var productFamilyByName =await  _cosmosDbContext.productFamily.ToListAsync();
            var selectedProductFamily = productFamilyByName.FirstOrDefault(p => p.ProductFamilyName.ToLower() == Name.ToLower());
            return selectedProductFamily;
        }
        public async Task TempRemoveProductFamily(List<string> ProductFamilyIdsToDelete)
        {
            try
            {
                if (ProductFamilyIdsToDelete == null)
                {
                    _logger.LogError("ProductFamilyService-RemoveProductFamily: Input value cannot be empty");
                    return;
                }

                List<DeletedProductFamily> deletedProductFamilyModel = new List<DeletedProductFamily>();
                foreach (var id in ProductFamilyIdsToDelete)
                {
                    //using (var _cosmosDbContextTransaction = _cosmosDbContext.Database.BeginTransaction())
                    //{
                    try
                    {
                        _logger.LogInfo($"DealerService-RemoveDealer: [Started] removing Dealer for id [{id}] on Azure AD B2C");
                        var productFamily = await _cosmosDbContext.productFamily.FindAsync(id);
                        if (productFamily != null)
                        {
                            var deletedPf = GraphClientUtility.ConvertProductFamillyToDeleteProductFamily(productFamily, _logger);
                            await _cosmosDbContext.deletedProductFamilies.AddAsync(deletedPf); ;
                            _cosmosDbContext.SaveChanges();
                            var resultDelete = _cosmosDbContext.productFamily.Remove(productFamily);
                            _cosmosDbContext.SaveChanges();
                            //_cosmosDbContextTransaction.Commit();
                            _logger.LogInfo($"ProductFamilyService-RemoveProducFamily: [Completed] removed ProductFamily [{id}] on Azure Cosmos B2C");
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ProductFamilyService-RemoveProductFamily: Exception occured while removing ProductFamily for id [{id}]");
                        _logger.LogError(ex);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyService-RemoveProductFamily: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }
        public async Task RemoveProductFamilyPermanently(List<string> productFamilysToDelete)
        {
            try
            {
                if (productFamilysToDelete == null)
                {
                    _logger.LogError("ProductFamilyService-RemoveProduct: Input value cannot be empty");
                    return;
                }

                foreach (var id in productFamilysToDelete)
                {
                    try
                    {
                        _logger.LogInfo($"ProductFamilyService-RemoveProductFamily: [Started] removing Dealer for id [{id}] on Azure Cosmos DB B2C");
                        var deletedPF = await _cosmosDbContext.deletedProductFamilies.FindAsync(id);
                        var delteResult = _cosmosDbContext.deletedProductFamilies.Remove(deletedPF);
                        _cosmosDbContext.SaveChanges();
                        _logger.LogInfo($"ProductFamilyService-RemoveProductFamily: [Completed] removed ProductFamily [{id}] on Azure cosmos DB B2C");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"ProductFamilyService-RemoveProductFamily: Exception occured while removing ProductFamily for id [{id}]");
                        _logger.LogError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyService-RemoveProductFamily: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }
        public Task<List<DeletedProductFamily>> GetDeletedProductFamilies()
        {
            return _cosmosDbContext.deletedProductFamilies.ToListAsync();
            //return deletedDelaers
        }
        public Task<List<FileDetails>> GetFileDetailsByFileName()
        {
            return _cosmosDbContext.fileDetails.Where(f=>f.FileName== "ProductFamilyCreateTemplate.csv").ToListAsync();
        }
        public Task<DeletedProductFamily> GetDeletedProductFamilyById(string Id)
        {
            return _cosmosDbContext.deletedProductFamilies.Where(d => d.ProductFamilyId == Id).SingleOrDefaultAsync();
        }
        public async Task<bool> RemovePFinRestoreById(string Id)
        {
            try
            {
                if (Id == null)
                {
                    _logger.LogError("ProductFamilyService-RemoveProductFamily: Input value cannot be empty");
                }

                _logger.LogInfo($"ProductFamilyService-RemoveProductFamily: [Started] removing ProductFamily for id [{Id}] on Azure Cosmos B2C");
                var deltedPF = await _cosmosDbContext.deletedProductFamilies.FindAsync(Id);
                var deletedResult = _cosmosDbContext.deletedProductFamilies.Remove(deltedPF);
                _cosmosDbContext.SaveChanges();
                _logger.LogInfo($"ProductFamilyService-RemoveDealer: [Completed] Restore ProductFamily [{Id}] on Azure Cosmos B2C");
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError("ProductFamilyService-RemoveProductFamily: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }
    }
}
