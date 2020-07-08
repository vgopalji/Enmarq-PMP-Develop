using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Utility
{
    //Add Logs
    public static class GraphClientUtility
    {
        #region Variables

        public static string TenantId;
        public static string b2cExtensionAppClientId;
        public static string AADGraphResourceId;
        public static string AADGraphVersion;
        public static string ExtensionName;
        public static string ApplicationName;
        public static string AppObjectId;

        private static IConfiguration configuration;

        private static string appId;
        private static string clientSecret;

        private static string aadInstance;
        private static string graphResource;
        private static string graphAPIEndpoint;
        private static string authority;
        public static string Domain;

        #endregion

        #region Constructor
        static GraphClientUtility()
        {
            configuration = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
            SetAzureADBB2COptions();
        }

        private static void SetAzureADBB2COptions()
        {
            var azureOptions = new AzureADB2C();
            configuration.Bind("AzureADB2C", azureOptions);

            TenantId = azureOptions.TenantId;
            appId = azureOptions.AppId;
            clientSecret = azureOptions.ClientSecret;
            //b2cExtensionAppClientId = azureOptions.B2cExtensionAppClientId;

            graphResource = azureOptions.GraphResource;
            graphAPIEndpoint = $"{azureOptions.GraphResource}{azureOptions.GraphResourceEndPoint}";

            aadInstance = azureOptions.Instance;
            authority = $"{aadInstance}{TenantId}";

            AADGraphResourceId = azureOptions.AADGraphResourceId;
            AADGraphVersion = azureOptions.AADGraphVersion;
            ExtensionName = azureOptions.ExtensionName;
            ApplicationName = azureOptions.ApplicationName;
            AppObjectId = azureOptions.AppObjectId;
            Domain = azureOptions.Domain;

        }
        #endregion

        #region Get Graph Client
        public static GraphServiceClient GetGraphServiceClient()
        {
            GraphServiceClient graphClient = null;
            try
            {
                if (string.IsNullOrEmpty(appId) && string.IsNullOrEmpty(TenantId) && string.IsNullOrEmpty(clientSecret))
                {
                    //Add Logger
                    return graphClient;
                }

                IConfidentialClientApplication clientApplication = ConfidentialClientApplicationBuilder
                                                                    .Create(appId)
                                                                    .WithTenantId(TenantId)
                                                                    .WithClientSecret(clientSecret)
                                                                    .WithClientVersion("V2")
                                                                    .Build();

                ClientCredentialProvider clientCredentialProvider = null;
                if (clientApplication != null)
                {
                    clientCredentialProvider = new ClientCredentialProvider(clientApplication);
                }

                if (clientCredentialProvider != null)
                {
                    graphClient = new Microsoft.Graph.GraphServiceClient(clientCredentialProvider);
                }

            }
            catch (Exception ex)
            {
                //Todo: Add Logger
                throw ex;
            }
            return graphClient;
        }
        #endregion

        #region Extension 

        public async static Task<Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult> GetAuthentication()
        {
            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult authenticationResult;
            try
            {
                if (string.IsNullOrEmpty(authority) && string.IsNullOrEmpty(appId) && string.IsNullOrEmpty(clientSecret) && string.IsNullOrEmpty(AADGraphResourceId))
                {
                    //Add Logger
                    return null;
                }

                AuthenticationContext authContext = new AuthenticationContext(authority);
                Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential credential = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(appId, clientSecret);

                authenticationResult = await authContext.AcquireTokenAsync(AADGraphResourceId, credential);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return authenticationResult;
        }

        #endregion

        #region Convert Graph User Object to User Model Object

        public static UserModel ConvertGraphUserToUserModel(User graphUser, ILoggerManager logger)
        {
            UserModel retVal = null;
            try
            {
                if (graphUser != null)
                {
                    retVal = new UserModel
                    {
                        Groups = new List<string>(),
                        RolesAA = new List<string>(),
                        CustomAttributes = new Dictionary<string, string>(),


                        Address2 = string.Empty,
                        Address3 = string.Empty,
                        BusinessEmail = string.Empty,
                        Password = string.Empty,
                        AutoGeneratePassword = false,
                        ManualCreatedPassword = false,
                        DelegateAdministration = false,
                        Region = string.Empty,
                        SignInName = string.Empty,

                        AccountEnabled = graphUser.AccountEnabled,
                        UsageLocation = graphUser.UsageLocation,

                        Id = graphUser.Id,
                        Address = graphUser.StreetAddress, // Add Code to split the address 
                        StreetAddress = graphUser.StreetAddress,
                        GivenName = graphUser.GivenName,
                        DisplayName = graphUser.DisplayName,
                        Department = graphUser.Department,
                        CompanyName = graphUser.CompanyName,
                        Country = graphUser.Country,
                        CreationType = graphUser.CreationType,
                        Mail = graphUser.Mail,
                        MobilePhone = graphUser.MobilePhone,
                        OfficeLocation = graphUser.OfficeLocation,
                        PostalCode = graphUser.PostalCode,
                        PreferredName = graphUser.PreferredName,
                        State = graphUser.State,
                        Surname = graphUser.Surname,
                        UserPrincipalName = graphUser.UserPrincipalName,
                        UserType = graphUser.UserType,
                        JobTitle = graphUser.JobTitle,
                        BusinessPhones = graphUser.BusinessPhones,
                        WebSite = graphUser.MySite
                    };

                }
            }
            catch (Exception ex)
            {
                logger.LogError("GraphClientUtility-ConvertGraphUserToUserModel: Exception occurred....");
                logger.LogError(ex);
            }
            return retVal;
        }

        public static UserModel ConvertDeletedGraphUserToUserModel(dynamic graphUser, ILoggerManager logger)
        {
            UserModel retVal = null;
            try
            {
                if (graphUser != null)
                {
                    var clientArray = graphUser.Value<JArray>("businessPhones");
                    var businessPhones = clientArray.ToObject<List<string>>();
                    retVal = new UserModel
                    {
                        Groups = new List<string>(),
                        RolesAA = new List<string>(),
                        CustomAttributes = new Dictionary<string, string>(),

                        BusinessPhones = businessPhones,
                        Address2 = string.Empty,
                        Address3 = string.Empty,
                        BusinessEmail = string.Empty,
                        Password = string.Empty,
                        AutoGeneratePassword = false,
                        ManualCreatedPassword = false,
                        DelegateAdministration = false,
                        Region = string.Empty,
                        SignInName = string.Empty,

                        AccountEnabled = false,
                        UsageLocation = string.Empty,

                        Id = graphUser.Value<string>("id"),
                        Address = string.Empty,
                        StreetAddress = string.Empty,
                        GivenName = graphUser.Value<string>("givenName"),
                        DisplayName = graphUser.Value<string>("displayName"),
                        Department = string.Empty,
                        CompanyName = graphUser.Value<string>("companyName"),
                        Country = string.Empty,
                        CreationType = string.Empty,
                        Mail = graphUser.Value<string>("mail"),
                        MobilePhone = graphUser.Value<string>("mobilePhone"),
                        OfficeLocation = graphUser.Value<string>("officeLocation"),
                        PostalCode = string.Empty,
                        PreferredName = string.Empty,
                        Language_C = graphUser.Value<string>("preferredLanguage"),
                        State = string.Empty,
                        Surname = graphUser.Value<string>("surname"),
                        UserPrincipalName = graphUser.Value<string>("userPrincipalName"),
                        UserType = string.Empty,
                        JobTitle = graphUser.Value<string>("jobTitle"),
                        WebSite = string.Empty
                    };

                }
            }
            catch (Exception ex)
            {
                logger.LogError("GraphClientUtility-ConvertGraphUserToUserModel: Exception occurred....");
                logger.LogError(ex);
            }
            return retVal;
        }


        #endregion

        #region Convert Graph Group Object to Group Model Object

        public static GroupModel ConvertGraphGroupToGroupModel(Group graphGroup, ILoggerManager logger)
        {
            GroupModel retVal = null;
            try
            {
                if (graphGroup != null)
                {
                    retVal = new GroupModel
                    {
                        CreatedDateTime = graphGroup.CreatedDateTime,
                        ObjectId = graphGroup.Id,
                        DisplayName = graphGroup.DisplayName,
                        GroupTypes = graphGroup.GroupTypes,
                        MailEnabled = graphGroup.MailEnabled,
                        MailNickname = graphGroup.MailNickname,
                        SecurityEnabled = graphGroup.SecurityEnabled,
                        Visibility = graphGroup.Visibility,
                        Description = graphGroup.Description,
                        AllowExternalSenders = graphGroup.AllowExternalSenders,
                        AutoSubscribeNewMembers = graphGroup.AutoSubscribeNewMembers,
                        AdditionalData = new Dictionary<string, object>()
                    };

                    if (retVal.GroupTypes != null)
                    {
                        if (retVal.GroupTypes.Any(x => x == CareStreamConst.DynamicMembership))
                        {
                            //Nothing to do
                        }
                        else if (retVal.GroupTypes.Any(x => x == CareStreamConst.Unified))
                        {
                            retVal.GroupTypes = null;
                            retVal.GroupTypes = new List<string>
                            {
                                CareStreamConst.O365
                            };
                        }
                        else
                        {
                            retVal.GroupTypes = null;
                            retVal.GroupTypes = new List<string>
                            {
                                CareStreamConst.Security
                            };
                        }

                        if (retVal.GroupTypes.Any())
                        {
                            retVal.GroupType = string.Join(CareStreamConst.Pipe, retVal.GroupTypes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("GraphClientUtility-ConvertGraphUserToUserModel: Exception occurred....");
                logger.LogError(ex);
            }
            return retVal;
        }


        public static DeletedDealerModel ConvertDealerToDeleteDealer(DealerModel dealerModel, ILoggerManager logger)
        {
           

            //if (dealerModel != null)
            //{
                DeletedDealerModel ddm = new DeletedDealerModel();

                ddm.DealerId = dealerModel.DealerId;
                ddm.DealerName = dealerModel.DealerName;
                ddm.DealerDescription = dealerModel.DealerDescription;
                ddm.SAPID = dealerModel.SAPID;
                ddm.deletedDealerProductFamilyModels = new List<DeletedDealerProductFamilyModel>();

                try
                {
                    foreach (var deltedPFM in dealerModel.assignedProductFamilyModels)
                    {
                        if (deltedPFM.ProductFamilyId != null)
                        {
                            var parseBranch = ParseProductFamilyToDeltePF(deltedPFM,logger);
                            ddm.deletedDealerProductFamilyModels.Add(parseBranch);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("GraphClientUtility-ConvertGraphUserToUserModel: Exception occurred....");
                    logger.LogError(ex);
                }

                return ddm;
            //}


        }
    


public static DeletedDealerProductFamilyModel ParseProductFamilyToDeltePF(AssignedProductFamilyModel productFamilyModel, ILoggerManager logger)
{
            DeletedDealerProductFamilyModel ddpf = null;
    try
    {
        if (productFamilyModel != null)
        {
                    ddpf = new DeletedDealerProductFamilyModel
            {
                ProductFamilyId = productFamilyModel.ProductFamilyId,
                ProductFamilyName = productFamilyModel.ProductFamilyName,
                ProductDescription = productFamilyModel.ProductDescription              

            };

        }

    }
    catch (Exception ex)
    {
        logger.LogError("GraphClientUtility-ConvertGraphUserToUserModel: Exception occurred....");
        logger.LogError(ex);
    }
    return ddpf;
}

        //public static DealerModelVm ConvertDealerToDealerVm(DealerModelCosmos dealerModel, ILoggerManager logger)
        //{
        //    DealerModelVm retVal = null;
        //    try
        //    {
        //        if (dealerModel != null)
        //        {
        //            retVal = new DealerModelVm
        //            {
        //                DealerId = dealerModel.DealerId,
        //                DealerName = dealerModel.DealerName,
        //                DealerDescription = dealerModel.DealerDescription,
        //                SAPID = dealerModel.SAPID,                       

        //            };

        //            }

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError("GraphClientUtility-ConvertGraphUserToUserModel: Exception occurred....");
        //        logger.LogError(ex);
        //    }
        //    return retVal;
        //}
        #endregion
    }
}
