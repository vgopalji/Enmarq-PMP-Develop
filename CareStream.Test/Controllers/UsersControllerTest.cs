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
    public class UsersControllerTest
    {
        private UsersController _usersController;
        private Mock<IUserService> mockUserServiceRepo;
        private Mock<IUserAttributeService> mockUserAttributeServiceRepo;

        #region Test Cases

        [Fact]
        public async Task Index_GetUsers_ReturnsAViewResult_WithUsersList()
        {
            DoUserControllerInitialSetup();
            mockUserServiceRepo.Setup(x => x.GetUser()).Returns(Task.FromResult(GetUserModel()));

            // Act
            var result = await _usersController.UsersList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as UsersModel;
            Assert.Single(model.Users);
        }

        [Fact]
        public async Task DeletedUsers_ReturnsAViewResult_WithUsersList()
        {
            DoUserControllerInitialSetup();
            mockUserServiceRepo.Setup(x => x.GetDeletedUser()).Returns(Task.FromResult(GetUserModel()));

            // Act
            var result = await _usersController.GetDeletedUsers();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.Value) as UsersModel;
            Assert.Single(model.Users);
        }

        [Fact]
        public async Task Create_ReturnsAViewResult_WithUserModel()
        {
            DoUserControllerInitialSetup();
            mockUserServiceRepo.Setup(x => x.GetUserDropDownAsync()).Returns(Task.FromResult(GetUserDropDownModel()));

            // Act
            var result = await _usersController.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<object>(viewResult.ViewData.Model) as UserViewModel;
            Assert.IsType<UserModel>(model.UserModel);
        }

        [Fact]
        public async Task GetFilteredUsers_ReturnsAViewResult_WithUsersList()
        {
            DoUserControllerInitialSetup();
            mockUserServiceRepo.Setup(x => x.GetFilteredUsers(It.IsAny<string>())).Returns(Task.FromResult(GetUserModel()));

            // Act
            var result = await _usersController.GetFilteredUsers("test");

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            var users = Assert.IsAssignableFrom<object>(viewResult.RouteValues["Users"]) as List<UserModel>;
            Assert.Single(users);
        }

        [Fact]
        public async Task CreateInvite_With_UserViewModel()
        {
            DoUserControllerInitialSetup();
            mockUserServiceRepo.Setup(x => x.SendInvite(It.IsAny<InviteUser>())).Returns(Task.FromResult(GetUserModel()));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _usersController.TempData = tempData;

            // Act
            var result = await _usersController.CreateInvite(GetUserViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_usersController.TempData["UserMessage"]);
        }

        [Fact]
        public async Task UserDelete_With_Selected_Users()
        {
            DoUserControllerInitialSetup();
            mockUserServiceRepo.Setup(x => x.RemoveUser(It.IsAny<List<string>>())).Returns(Task.FromResult(GetUserModel()));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _usersController.TempData = tempData;

            // Act
            var result = await _usersController.UserDelete(new List<string> { "test" });

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_usersController.TempData["UserMessage"]);
        }

        [Fact]
        public async Task Post_ReturnsAViewResult_ByCreatingUser()
        {
            DoUserControllerInitialSetup();

            mockUserServiceRepo.Setup(x => x.Create(It.IsAny<UserModel>())).Returns(Task.FromResult(true));
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _usersController.TempData = tempData;

            // Act
            var result = await _usersController.Post(GetUserViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(_usersController.TempData["UserMessage"]);
        }

        #endregion

        #region Private Methods

        private void DoUserControllerInitialSetup()
        {
            mockUserServiceRepo = new Mock<IUserService>();
            mockUserAttributeServiceRepo = new Mock<IUserAttributeService>();
            var mockLoggerRepo = new Mock<ILoggerManager>();

            _usersController = new UsersController(mockUserServiceRepo.Object, mockLoggerRepo.Object, mockUserAttributeServiceRepo.Object);
        }

        private UsersModel GetUserModel()
        {
            return new UsersModel
            {
                Users = new List<UserModel>
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

        private UserDropDownModel GetUserDropDownModel()
        {
            return new UserDropDownModel
            {
                AutoPassword = PasswordService.GenerateNewPassword(GetRandomNumber(), GetRandomNumber(), GetRandomNumber(), GetRandomNumber()),
                Groups = new Dictionary<string, string> { { "TestGroup", "Test Group" } },
                UserRoles = new List<UserRole> { new UserRole { Key = "Admin", Value = "App Admin" } },
                UserBusinessDepartments = new List<UserBusinessDepartment> { new UserBusinessDepartment { Key = "TestBussDept", Value = "Test Dept" } },
                UserLanguages = new List<UserLanguage> { new UserLanguage { Key = "Eng", Value = "English" } },
                UserLocation = new List<Country> { new Country { CountryCode = "US", CountryName = "USA", Region = "CA" } },
                UserTypes = new List<UserType> { new UserType { Key = "TestType", Value = "Test Type" } }
            };
        }

        private int GetRandomNumber()
        {
            Random random = new Random();
            return random.Next(1, 10);
        }

        private UserViewModel GetUserViewModel()
        {
            return new UserViewModel
            {
                UserModel = new UserModel
                {
                    Id = "TestUser",
                    UserPrincipalName = "test@contoso.com",
                    DisplayName = "Chris Red",
                    GivenName = "Chris Red",
                    PasswordOptions = new List<Models.User.PasswordOption>
                    {
                        new Models.User.PasswordOption
                        {
                            Id = 1,
                            Type = Models.User.PasswordOptionType.AutoGeneratePassword
                        }
                    },
                    Password = "testPassword@123",
                    Mail = "test@contoso.com"
                },

                InviteUser = new InviteUser
                {
                    Status = "Completed",
                    InvitedUser = new Microsoft.Graph.User
                    {
                        Id = "TestUser",
                        UserPrincipalName = "test@contoso.com",
                        DisplayName = "Chris Red",
                        GivenName = "Chris Red",
                        Country = "USA",
                        Mail = "test@contoso.com"
                    }
                }
            };
        }

        #endregion
    }
}
