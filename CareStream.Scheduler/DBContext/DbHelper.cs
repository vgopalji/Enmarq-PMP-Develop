using CareStream.LoggerService;
using CareStream.Models;
using System;


namespace CareStream.Scheduler
{
    public class DbHelper
    {
        private readonly ILoggerManager _logger;
        private readonly CareStreamContext _cc;
        public DbHelper(ILoggerManager logger, CareStreamContext cc)
        {
            _logger = logger;
            _cc = cc;
        }

        public void UpdateTable(BulkUser bulkUser, string status)
        {
            try
            {
                if (_cc != null)
                {
                    bulkUser.Status = status;
                    bulkUser.ModifiedDate = DateTime.UtcNow;
                    _cc.Update(bulkUser);
                    _cc.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"DbHelper-UpdateTable:Exception occured while updating the bulk user id {bulkUser.Id}");
                _logger.LogError(ex);
            }
        }

        public void UpdateTable(BulkUserFile bulkUserFile, string status)
        {
            try
            {
                if (_cc != null)
                {
                    bulkUserFile.Status = status;
                    _cc.Update(bulkUserFile);
                    _cc.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"DbHelper-UpdateTable:Exception occured while updating the bulk user file for id {bulkUserFile.Id}");
                _logger.LogError(ex);
            }
        }

        public static CareStreamContext GetCareStreamContext()
        {
            CareStreamContextFactory apiContextFactory = new CareStreamContextFactory();

            return apiContextFactory.CreateDbContext(new string[] { });
        }
    }
}
