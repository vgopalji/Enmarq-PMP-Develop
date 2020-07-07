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
    public class GroupMembersControllerTest
    {
        private GroupMembersController _groupMembersController;
        private Mock<IGroupMemberService> mockGroupMemberServiceRepo;

        #region Test Cases

        [Fact]
        public async Task Index_ReturnsAViewResult_WithGroupMemberModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockGroupMemberServiceRepo.Setup(x => x.GetGroupMembers(It.IsAny<string>())).Returns(Task.FromResult(GetGroupMemberModel()));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _groupMembersController.TempData = tempData;

            // Act
            var result = await _groupMembersController.Index("test");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as GroupMemberModel;
            Assert.Equal("test", model.GroupId);
        }

        [Fact]
        public async Task AddMemberAsync_ReturnsARedirectToActionResult_ByAddingGroupMemberModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockGroupMemberServiceRepo.Setup(x => x.AddGroupMembers(It.IsAny<GroupMemberAssignModel>())).Returns(Task.FromResult(true));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData["GroupId"] = "test";
            _groupMembersController.TempData = tempData;

            // Act
            var result = await _groupMembersController.AddMemberAsync("test", new List<string> { "test" });

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task GroupMemberDelete_ReturnsARedirectToActionResult_ByRemovingGroupMemberModel()
        {
            DoGroupMembersControllerInitialSetup();
            mockGroupMemberServiceRepo.Setup(x => x.RemoveGroupMembers(It.IsAny<GroupMemberAssignModel>())).Returns(Task.FromResult(true));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            tempData["GroupId"] = "test";
            _groupMembersController.TempData = tempData;

            // Act
            var result = await _groupMembersController.GroupMemberDelete(new List<string> { "test" });

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
        }

        #endregion

        #region Private Methods

        private void DoGroupMembersControllerInitialSetup()
        {
            mockGroupMemberServiceRepo = new Mock<IGroupMemberService>();
            var mockLoggerRepo = new Mock<ILoggerManager>();

            _groupMembersController = new GroupMembersController(mockGroupMemberServiceRepo.Object, mockLoggerRepo.Object);
        }

        private GroupMemberModel GetGroupMemberModel()
        {
            return new GroupMemberModel
            {
                GroupId = "test",
                Members = new Dictionary<string, string> { { "testKey", "testValue" } },
                AssignedMembers = new List<UserModel>
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
