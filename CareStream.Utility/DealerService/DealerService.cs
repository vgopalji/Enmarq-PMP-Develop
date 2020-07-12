using CareStream.LoggerService;
using CareStream.Models;
using CareStream.Models.Dealer;
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
        Task RemoveDealer(List<string> dealerIdsToDelete);
        Task TempRemoveDealer(List<string> dealerIdsToDelete);
        Task<DealerModel> GetDealerById(string Id);
        Task<List<DeletedDealerModel>> GetDeletedDealers();
        Task RemoveDealerPermanently(List<string> dealerIdsToDelete);
        Task<bool> AddFileDetails(FileDetails fileDetails);
        Task<List<FileDetails>> GetFileDetails();
        Task<List<DealerModel>> GetAllDealers();
        Task<DealerModel> GetDealeryByName(string name);
        Task<bool> RemoveDealerInRestoreById(string Id);
        Task<DeletedDealerModel> GetDeletedDealersById(string Id);
    }

    public class DealerService : CosmosDBService<DealerModel>, IDealerService
    {
        private readonly ILoggerManager _logger;
        private readonly CosmosDbContext _cosmosDbContext;

        public DealerService(ILoggerManager logger, CosmosDbContext cosmosDbContext) : base(cosmosDbContext)
        {
            _logger = logger;
            _cosmosDbContext = cosmosDbContext;

        }
      
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
        public async Task<bool> AddFileDetails(FileDetails fileDetails)
        {
            try
            {
                await _cosmosDbContext.fileDetails.AddAsync(fileDetails);
                _cosmosDbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            //return true;

        }
        public Task<List<FileDetails>> GetFileDetails()
        {
            return _cosmosDbContext.fileDetails.Where(f=>f.FileName=="DealerCreateTemplate.csv").ToListAsync();
        }
        public Task<List<DeletedDealerModel>> GetDeletedDealers()
        {
            return _cosmosDbContext.deletedDealerModels.ToListAsync();
            //return deletedDelaers
        }
        public Task<DeletedDealerModel> GetDeletedDealersById(string Id)
        {
            return _cosmosDbContext.deletedDealerModels.Where(d => d.DealerId == Id).SingleOrDefaultAsync();
        }
        public Task<List<DealerModel>> GetAllDealers()
        {
            var dealers = _cosmosDbContext.dealers.ToListAsync();
            return dealers;
        }
        public async Task<DealerModel> GetDealeryByName(string name)
        {
            var dealerList = await _cosmosDbContext.dealers.ToListAsync();
            var dealer = dealerList.FirstOrDefault(p => p.DealerName.ToLower() == name.ToLower());
            return dealer;
        }
       public async Task<bool> RemoveDealerInRestoreById(string Id)
        {
            try
            {
                if (Id == null)
                {
                    _logger.LogError("DealerService-RemoveDealer: Input value cannot be empty");
                }

                        _logger.LogInfo($"DealerService-RemoveDealer: [Started] removing Dealer for id [{Id}] on Azure Cosmos B2C");
                        var deltedDealers = await _cosmosDbContext.deletedDealerModels.FindAsync(Id);
                        var deletedResult = _cosmosDbContext.deletedDealerModels.Remove(deltedDealers);
                        _cosmosDbContext.SaveChanges();
                _logger.LogInfo($"DealerService-RemoveDealer: [Completed] Restore Dealer [{Id}] on Azure Cosmos B2C");
                return true;
                
            }
            catch (Exception ex)
            {
                _logger.LogError("DealerService-RemoveDealer: Exception occured....");
                _logger.LogError(ex);
                throw ex;
            }
        }
    }


}
