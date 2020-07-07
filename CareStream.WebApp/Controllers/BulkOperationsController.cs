using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareStream.LoggerService;
using CareStream.Models.BulkFile;
using CareStream.Scheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CareStream.WebApp.Controllers
{
    public class BulkOperationsController : BaseController
    {
        private readonly ILoggerManager _logger;
        private readonly IBulkOperationService _bulkOperationService;

        public BulkOperationsController(ILoggerManager logger, IBulkOperationService bulkOperationService)
        {
            _logger = logger;
            _bulkOperationService = bulkOperationService;
        }

        public IActionResult FilesUploaded()
        {
            try
            {
                var fileModel = _bulkOperationService.GetUserFiles();

                return View(fileModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BulkOperationsController:FilesUploaded:Exception occured while getting the bulk user files...");
                _logger.LogError(ex);

                throw ex;
            }
        }

        public IActionResult Users(long Id)
        {
            try
            {
                var userModels = _bulkOperationService.GetFileUsers(Id);

                return View(userModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BulkOperationsController:Users:Exception occured while getting the bulk user files...");
                _logger.LogError(ex);

                throw ex;
            }
        }
    }
}
