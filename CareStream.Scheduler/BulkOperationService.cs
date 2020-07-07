using CareStream.Models.BulkFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CareStream.Scheduler
{
    public interface IBulkOperationService
    {
        List<UserFile> GetUserFiles();

        List<Users> GetFileUsers(long fileId);
    }

    public class BulkOperationService : IBulkOperationService
    {
        public List<Users> GetFileUsers(long fileId)
        {
            var dbContext = DbHelper.GetCareStreamContext();

            return (from buf in dbContext.BulkUsers
                    where buf.FileId == fileId
                    select new Users
                    {
                        Id = buf.Id,
                        Action = buf.Action,
                        FileId = buf.FileId,
                        UserPrincipalName = buf.UserPrincipalName,
                        Status = buf.Status,
                        CreatedDate = buf.CreatedDate
                    }).ToList();
        }

        public List<UserFile> GetUserFiles()
        {
            var dbContext = DbHelper.GetCareStreamContext();

            return (from buf in dbContext.BulkUserFiles
                    select new UserFile
                    {
                        Id = buf.Id,
                        Action = buf.Action,
                        FileName = buf.FileName,
                        Status = buf.Status,
                        CreatedDate = buf.CreatedDate
                    }).ToList();
        }
    }
}
