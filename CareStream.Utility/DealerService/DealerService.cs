using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Utility.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

namespace CareStream.Utility.DealerService
{
    public interface IDealerService : ICosmosDbService<DealerModel>
    {

        ////Task<List<DealerAssignModel>> BuildProductDealer();
        //List<DealerModelCosmos> GetDealerDetails();
        ////Task<List<DealerModelCosmos>> GetDealerDetailsCosmos();
        ////Task<List<DealerModelVm>> GetDealerDetailsCosmos();
        Task RemoveDealer(List<string> dealerIdsToDelete);
        Task TempRemoveDealer(List<string> dealerIdsToDelete);
        //Task<List<DealerModelVm>> GetAllDealerDetailsCosmos();
        //Task<DealerAssignVm> BuildProductDealer();
        Task<DealerModel> GetDealerById(string Id);
        Task<List<DeletedDealerModel>> GetDeletedDealers();
        Task RemoveDealerPermanently(List<string> dealerIdsToDelete);
    }

    //public class ProductFamilyService : CosmosDBService<ProductFamilyModel>, IProductFamilyService
    public class DealerService : CosmosDBService<DealerModel>, IDealerService
    {
        private readonly ILoggerManager _logger;
        private readonly CosmosDbContext _cosmosDbContext;

        public DealerService(ILoggerManager logger, CosmosDbContext cosmosDbContext) : base(cosmosDbContext)
        {
            _logger = logger;
            _cosmosDbContext = cosmosDbContext;

        }

        //    //public async Task<List<DealerModelVm>> GetDealerDetailsCosmos()
        //    //{
        //    //    var res = await _cosmosDbContext.dealerCosmos.ToListAsync();
        //    //    return res;
        //    //}


        //    #region MVC Controller Helpers
        //    //public async Task<List<DealerAssignModel>> BuildProductDealer()
        //    //{
        //    //    var dealerAssignModel = new List<DealerModelCosmos>();
        //    //    try
        //    //    {
        //    //        _logger.LogInfo("ProductFamilyService-BuildProductDealer: [Started] to get detail list of product family to build dealer");
        //    //        var dealers = GetAllDealerDetails();
        //    //        if (dealers != null)
        //    //        {
        //    //            DealerModelCosmos dealerAssignModelLocal = new DealerModelCosmos();
        //    //            foreach (var aDict in dealers.OrderBy(x => x.Value))
        //    //            {
        //    //                //  dealerAssignModelLocal..Add(aDict.Key, aDict.Value);
        //    //            }
        //    //            dealerAssignModel.Add(dealerAssignModelLocal);

        //    //        }
        //    //        _logger.LogInfo("ProductFamilyService-BuildProductDealer: [Completed] to getting detail list of user to build Dealer");

        //    //    }
        //    //    catch (ServiceException ex)
        //    //    {
        //    //        _logger.LogError("ProductFamilyService-BuildProductDealer: Exception occured....");
        //    //        _logger.LogError(ex);
        //    //    }
        //    //    return dealerAssignModel;
        //    //}



        //    public List<DealerModelCosmos> GetDealerDetails()
        //    {
        //        try
        //        {
        //            List<DealerModelCosmos> dealerList = new List<DealerModelCosmos>();
        //            int count = 0;
        //            string jsonString = System.IO.File.ReadAllText("D:\\New folder\\CareStream.Utility\\App_Data\\Dealer.json");

        //            using (JsonDocument document = JsonDocument.Parse(jsonString))
        //            {
        //                JsonElement root = document.RootElement;
        //                JsonElement productFamilyElement = root.GetProperty("Dealers");

        //                count = productFamilyElement.GetArrayLength();

        //                foreach (JsonElement productFamily in productFamilyElement.EnumerateArray())
        //                {
        //                    if (productFamily.TryGetProperty("dealerId", out JsonElement dealerId) && productFamily.TryGetProperty("dealerName", out JsonElement dealerName) && productFamily.TryGetProperty("dealerDescription", out JsonElement dealerDescription) && productFamily.TryGetProperty("SAPID", out JsonElement SAPID))
        //                    {
        //                        DealerModelCosmos dealer = new DealerModelCosmos();
        //                        //sum += gradeElement.GetDouble();
        //                        dealer.DealerId = dealerId.GetString();
        //                        dealer.DealerName = dealerName.GetString();
        //                        dealer.DealerDescription = dealerDescription.GetString();
        //                        dealer.SAPID = SAPID.GetString();
        //                        dealerList.Add(dealer);
        //                    }

        //                    else
        //                    {
        //                        return null;
        //                    }
        //                }
        //            }
        //            if (dealerList == null)
        //            {
        //                return null;
        //            }
        //            return dealerList;
        //        }
        //        catch (ServiceException ex)
        //        {
        //            _logger.LogError("ProductFamily-GetDetailProductFamilyList: Exception occured....");
        //            _logger.LogError(ex);
        //            throw ex;
        //        }

        //    }

        //    //public async Task<TEntity> CreateDealerAsync(List<DealerModelCosmos> entity)
        //    //{

        //    //    //entity.ProductFamilyId = Guid.NewGuid().ToString();
        //    //    var response = _cosmosDbContext.dealerCosmos.Add(entity);
        //    //    await ctx.SaveChangesAsync();
        //    //    return response.Entity;

        //    //}


        //    #endregion

        //    #region Private Methods
        //    private Dictionary<string, string> GetAllDealerDetails()
        //    {
        //        Dictionary<string, string> dealerList = null;
        //        try
        //        {
        //            dealerList = new Dictionary<string, string>();
        //            int count = 0;
        //            string jsonString = System.IO.File.ReadAllText("D:\\New folder\\CareStream.Utility\\App_Data\\Dealer.json");

        //            using (JsonDocument document = JsonDocument.Parse(jsonString))
        //            {
        //                JsonElement root = document.RootElement;
        //                JsonElement productFamilyElement = root.GetProperty("Dealers");

        //                count = productFamilyElement.GetArrayLength();

        //                foreach (JsonElement productFamily in productFamilyElement.EnumerateArray())
        //                {
        //                    if (productFamily.TryGetProperty("dealerId", out JsonElement dealerId) && productFamily.TryGetProperty("dealerName", out JsonElement dealerName) && productFamily.TryGetProperty("dealerDescription", out JsonElement dealerDescription) && productFamily.TryGetProperty("SAPID", out JsonElement SAPID))
        //                    {
        //                        DealerModel dealer = new DealerModel();
        //                        //sum += gradeElement.GetDouble();
        //                        dealer.DealerId = dealerId.GetString();
        //                        dealer.DealerName = dealerName.GetString();
        //                        dealer.DealerDescription = dealerDescription.GetString();
        //                        dealer.SAPID = SAPID.GetString();
        //                        dealerList.Add(Convert.ToString(dealer.DealerId), dealer.DealerName);

        //                    }

        //                    else
        //                    {
        //                        return null;
        //                    }
        //                }

        //            }
        //            if (dealerList == null)
        //            {
        //                return null;
        //            }
        //            return dealerList;
        //        }
        //        catch (ServiceException ex)
        //        {
        //            _logger.LogError("ProductFamily-GetDetailProductFamilyList: Exception occured....");
        //            _logger.LogError(ex);
        //            throw ex;
        //        }

        //    }
        //    #endregion
        //    public async Task<List<DealerModelVm>> GetAllDealerDetailsCosmos()
        //    {

        //        var res = await _cosmosDbContext.dealerCosmos.ToListAsync();
        //        List<DealerModelVm> dmvm = new List<DealerModelVm>();
        //        foreach (var items in res)
        //        {
        //            var res1 = GraphClientUtility.ConvertDealerToDealerVm(items, _logger);
        //            dmvm.Add(res1);
        //        }
        //        return dmvm;

        //    }

        //    public async Task<DealerAssignVm> BuildProductDealer()
        //    {
        //        var dealerAssignModel = new DealerAssignVm();
        //        var productFamilyList =await _cosmosDbContext.productFamilyCosmos.ToListAsync();
        //        foreach(var res in productFamilyList)
        //        {
        //            dealerAssignModel.ProductFamilyDealer.Add(res);
        //        }
        //        return dealerAssignModel;
        //    }

        //    //private async Task<Dictionary<string, string>> GetNewGroupDefaultOwnerMember()
        //    //{
        //    //    Dictionary<string, string> retVal = null;
        //    //    try
        //    //    {
        //    //        var productFamily = _cosmosDbContext.productFamilyCosmos.ToListAsync();
        //    //        if (productFamily != null)
        //    //        {
        //    //            retVal = new Dictionary<string, string>();
        //    //            retVal.Add(productFamily.key, productFamily.value);
        //    //        }
        //    //    }
        //    //    catch { }
        //    //}
        public async Task RemoveDealer(List<string> dealerIdsToDelete)
        {
            try
            {
                if (dealerIdsToDelete == null)
                {
                    _logger.LogError("GroupService-RemoveGroup: Input value cannot be empty");
                    return;
                }

                foreach (var id in dealerIdsToDelete)
                {
                    try
                    {
                        _logger.LogInfo($"DealerService-RemoveDealer: [Started] removing Dealer for id [{id}] on Azure AD B2C");
                        var res = await _cosmosDbContext.dealers.FindAsync(id);
                        var ress = _cosmosDbContext.dealers.Remove(res);
                        _cosmosDbContext.SaveChanges();
                        _logger.LogInfo($"DealerService-RemoveDealer: [Completed] removed Dealer [{id}] on Azure AD B2C");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"DealerService-RemoveDealer: Exception occured while removing Dealer for id [{id}]");
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

        public async Task TempRemoveDealer(List<string> dealerIdsToDelete)
        {
            try
            {
                if (dealerIdsToDelete == null)
                {
                    _logger.LogError("DealerService-RemoveDealer: Input value cannot be empty");
                    return;
                }

                List<DeletedDealerModel> deletedDealerModel = new List<DeletedDealerModel>();
                foreach (var id in dealerIdsToDelete)
                {
                    //using (var _cosmosDbContextTransaction = _cosmosDbContext.Database.BeginTransaction())
                    //{
                        try
                        {
                            _logger.LogInfo($"DealerService-RemoveDealer: [Started] removing Dealer for id [{id}] on Azure AD B2C");
                            var dealer = await _cosmosDbContext.dealers.FindAsync(id);
                            if (dealer != null)
                            {
                                var res1 = GraphClientUtility.ConvertDealerToDeleteDealer(dealer, _logger);
                                await _cosmosDbContext.deletedDealerModels.AddAsync(res1); ;
                                _cosmosDbContext.SaveChanges();
                                var res = _cosmosDbContext.dealers.Remove(dealer);
                                _cosmosDbContext.SaveChanges();
                                //_cosmosDbContextTransaction.Commit();
                                _logger.LogInfo($"DealerService-RemoveDealer: [Completed] removed Dealer [{id}] on Azure AD B2C");
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"DealerService-RemoveDealer: Exception occured while removing Dealer for id [{id}]");
                            _logger.LogError(ex);
                        }
                    }
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError("GroupService-RemoveGroup: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }

        public async Task RemoveDealerPermanently(List<string> dealerIdsToDelete)
        {
            try
            {
                if (dealerIdsToDelete == null)
                {
                    _logger.LogError("GroupService-RemoveGroup: Input value cannot be empty");
                    return;
                }

                foreach (var id in dealerIdsToDelete)
                {
                    try
                    {
                        _logger.LogInfo($"DealerService-RemoveDealer: [Started] removing Dealer for id [{id}] on Azure AD B2C");
                        var res = await _cosmosDbContext.deletedDealerModels.FindAsync(id);
                        var ress = _cosmosDbContext.deletedDealerModels.Remove(res);
                        _cosmosDbContext.SaveChanges();
                        _logger.LogInfo($"DealerService-RemoveDealer: [Completed] removed Dealer [{id}] on Azure AD B2C");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"DealerService-RemoveDealer: Exception occured while removing Dealer for id [{id}]");
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
        public Task<DealerModel> GetDealerById(string Id)
        {
            var dealersById = _cosmosDbContext.dealers.Where(d => d.DealerId == Id).SingleOrDefaultAsync();
            return dealersById;
        }
   
    public Task<List<DeletedDealerModel>> GetDeletedDealers()
        {
            return _cosmosDbContext.deletedDealerModels.ToListAsync();
            //return deletedDelaers
        }
    }


}
