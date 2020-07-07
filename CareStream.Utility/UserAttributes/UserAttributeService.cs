using CareStream.LoggerService;
using CareStream.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareStream.Utility
{
    public interface IUserAttributeService
    {
        Task<UserAttributeModel> GetUserAttribute();

        Task UpsertUserAttributes(UserAttributeModel userAttribute);
    }

    public class UserAttributeService : IUserAttributeService
    {
        private readonly ILoggerManager _logger;

        public UserAttributeService(ILoggerManager logger)
        {
            _logger = logger;
        }
        public async Task<UserAttributeModel> GetUserAttribute()
        {
            var client = GraphClientUtility.GetGraphServiceClient();
            var userAttributes = new UserAttributeModel();

            var properties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
               .Request()
               .GetAsync();

            var extProperties = new List<Properties>();
            foreach (ExtensionProperty ext in properties.CurrentPage)
            {
                var names = ext.Name.Split('_');
                extProperties.Add(new Properties
                {
                    Name = names[names.Length - 1],
                    DataType = ext.DataType
                });
            }
            userAttributes.Properties = extProperties;
            return userAttributes;
        }

        public async Task<SchemaExtension> GetIfExtensionExist(string schemaName)
        {
            var retVal = new SchemaExtension();
            try
            {
                var client = GraphClientUtility.GetGraphServiceClient();

                if (client == null)
                {
                    _logger.LogError("UserAttributeService-CheckIfExtensionExist: Unable to create proxy for the Azure AD B2C graph client");
                    return retVal;
                }

                var schemaExtensions = await client.SchemaExtensions.Request().GetAsync();

                while (schemaExtensions.NextPageRequest != null)
                {
                    foreach (SchemaExtension extension in schemaExtensions.CurrentPage)
                    {
                        if (extension.Id.Contains(schemaName))
                        {
                            retVal = extension;
                            break;
                        }
                    }
                    if (retVal == null)
                    {
                        break;
                    }
                    schemaExtensions = await schemaExtensions.NextPageRequest.GetAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("UserAttributeService-CheckIfExtensionExist: Exception occured....");
                _logger.LogError(ex);
            }
            return retVal;
        }

        public async Task<ExtensionProperty> getCarestreamExtensionPropertyIfExists(string propertyName)
        {
            var client = GraphClientUtility.GetGraphServiceClient();

            var extProperties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
                            .Request()
                            .GetAsync();

            foreach (ExtensionProperty property in extProperties)
            {
                if (property.Name.Contains(propertyName))
                    return property;
            }

            return null;
        }

        public async Task UpsertUserAttributes(UserAttributeModel userAttribute)
        {
            var client = GraphClientUtility.GetGraphServiceClient();
           

           if (!await IsProertyExisty(userAttribute.Property.Name))
            {
                var extensionProperty = new ExtensionProperty();
                var properties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
               .Request()
               .GetAsync();
                extensionProperty = properties.CurrentPage.Where(prop => prop.AppDisplayName == userAttribute.AppDisplayName).First();

                extensionProperty = new ExtensionProperty
                {
                    Name = userAttribute.Property.Name,
                    DataType = userAttribute.Property.DataType,
                    TargetObjects = new List<String>()
                {
                    "User"
                }
                };

                await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
                    .Request()
                    .AddAsync(extensionProperty);
            }           

        }

        private async Task<Boolean> IsProertyExisty(string name)
        {
            var client = GraphClientUtility.GetGraphServiceClient();

            var properties = await client.Applications[GraphClientUtility.AppObjectId].ExtensionProperties
               .Request()
               .GetAsync();
            foreach (ExtensionProperty ext in properties.CurrentPage)
            {
                if (name.Equals(ext.Name))
                {
                    return true;
                }          
            }
            return false;
        }
    }
}
