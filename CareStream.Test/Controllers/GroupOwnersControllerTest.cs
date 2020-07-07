using CareStream.Models;
using CareStream.Utility;
using CareStream.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using CareStream.LoggerService;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace CareStream.Test.Controllers
{
    public class GroupOwnersControllerTest
    {
        private GroupOwnersController _groupOwnersController;
        private Mock<IGroupOwnerService> mockGroupOwnerServiceRepo;

        #region Test Cases

        [Fact]
        public async Task Index_ReturnsAViewResult_WithGroupOwnerModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockGroupOwnerServiceRepo.Setup(x => x.GetGroupOwners(It.IsAny<string>())).Returns(Task.FromResult(GetGroupOwnerModel()));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _groupOwnersController.TempData = tempData;

            // Act
            var result = await _groupOwnersController.Index("test");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as GroupOwnerModel;
            Assert.Equal("test", model.GroupId);
        }

        [Fact]
        public async Task AddOwnerAsync_ReturnsARedirectToActionResult_ByAddingGroupOwnerModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockGroupOwnerServiceRepo.Setup(x => x.AddGroupOwners(It.IsAny<GroupOwnerAssignModel>())).Returns(Task.FromResult(true));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData["GroupId"] = "test";
            _groupOwnersController.TempData = tempData;

            // Act
            var result = await _groupOwnersController.AddOwnerAsync("test", new List<string> { "test" });

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task GroupOwnerDelete_ReturnsARedirectToActionResult_ByRemovingGroupOwnerModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockGroupOwnerServiceRepo.Setup(x => x.RemoveGroupOwners(It.IsAny<GroupOwnerAssignModel>())).Returns(Task.FromResult(true));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData["GroupId"] = "test";
            _groupOwnersController.TempData = tempData;

            // Act
            var result = await _groupOwnersController.GroupOwnerDelete(new List<string> { "test" });

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        #endregion

        #region Private Methods

        private void DoGroupMembersControllerInitialSetup()
        {
            mockGroupOwnerServiceRepo = new Mock<IGroupOwnerService>();
            var mockLoggerRepo = new Mock<ILoggerManager>();

            _groupOwnersController = new GroupOwnersController(mockGroupOwnerServiceRepo.Object, mockLoggerRepo.Object);
        }

        private GroupOwnerModel GetGroupOwnerModel()
        {
            return new GroupOwnerModel
            {
                GroupId = "test",
                Owners = new Dictionary<string, string> { { "testKey", "testValue" } },
                AssignedOwners = new List<UserModel>
                {
                    new UserModel
                    {
                        Id = "TestUser",
                        UserPrincipalName="test@contoso.com",
                        DisplayName="Chris Red",
                        GivenName="Chris Red",
                        Password="testPassword@123",
                        Mail="test@contoso.com"
                    }
                }
            };
        }

        #endregion
    }
}
