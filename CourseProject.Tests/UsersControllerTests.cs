using Xunit;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using CourseProject.Web.Controllers;
using CourseProject.Web.ViewModels;
using CourseProject.BLL.Services;
using Domain.Models;
using System.Linq;

namespace CourseProject.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IAddressService> _mockAddressService;
        private readonly Mock<IRoleService> _mockRoleService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockAddressService = new Mock<IAddressService>();
            _mockRoleService = new Mock<IRoleService>();
            _mockMapper = new Mock<IMapper>();

            _controller = new UsersController(
                _mockUserService.Object,
                _mockAddressService.Object,
                _mockRoleService.Object,
                _mockMapper.Object
            );
        }

        #region Index Tests

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, Login = "User1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Phone = "+380123456789", AddressId = 1, RoleId = 1 },
                new User { UserId = 2, Login = "User2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", Phone = "+380987654321", AddressId = 2, RoleId = 2 }
            };

            var userViewModels = new List<UserViewModel>
            {
                new UserViewModel { UserId = 1, Login = "User1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Phone = "+380123456789", Address = "Street 1, City 1, Country 1", RoleName = "Admin" },
                new UserViewModel { UserId = 2, Login = "User2", Email = "user2@example.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", Phone = "+380987654321", Address = "Street 2, City 2, Country 2", RoleName = "User" }
            };

            _mockUserService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<UserViewModel>>(users)).Returns(userViewModels);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        #endregion

        #region Details Tests

        [Fact]
        public async Task Details_ExistingUser_ReturnsViewResult_WithUserViewModel()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId, Login = "User1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Phone = "+380123456789", AddressId = 1, RoleId = 1 };
            var userViewModel = new UserViewModel { UserId = userId, Login = "User1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Phone = "+380123456789", Address = "Street 1, City 1, Country 1", RoleName = "Admin" };

            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserViewModel>(user)).Returns(userViewModel);

            // Act
            var result = await _controller.Details(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.Equal("User1", model.Login);
        }

        [Fact]
        public async Task Details_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 99;
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Details(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        #endregion

        #region Create Tests

        [Fact]
        public void Create_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreatePost_ReturnsRedirectToActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var mockUserService = new Mock<IUserService>();
            var mockAddressService = new Mock<IAddressService>();
            var mockRoleService = new Mock<IRoleService>();
            var mockMapper = new Mock<IMapper>();

            var userViewModel = new UserViewModel
            {
                Login = "testuser",
                FirstName = "Test",
                LastName = "User",
                Gender = "Male",
                Email = "testuser@example.com",
                Phone = "+123456789",
                Address = "Street, City, Country",
                BankCardData = "1111-2222-3333-4444",
                RoleName = "Admin"
            };

            var user = new User
            {
                UserId = 1,
                Login = userViewModel.Login,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Gender = userViewModel.Gender,
                Email = userViewModel.Email,
                Phone = userViewModel.Phone,
                BankCardData = userViewModel.BankCardData
            };

            mockMapper.Setup(m => m.Map<User>(userViewModel)).Returns(user);

            // Setup для RegisterUserAsync
            mockUserService
                .Setup(s => s.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(user); // Повертає користувача

            var controller = new UsersController(mockUserService.Object, mockAddressService.Object, mockRoleService.Object, mockMapper.Object);

            // Act
            var result = await controller.Create(userViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            mockUserService.Verify(s => s.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
        }




        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewResult_WithModel()
        {
            // Arrange
            var userViewModel = new UserViewModel
            {
                // Missing required fields like Login, Email, etc.
            };
            _controller.ModelState.AddModelError("Login", "Required");

            // Act
            var result = await _controller.Create(userViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(userViewModel, model);
        }

        #endregion

        #region Edit Tests

        [Fact]
        public async Task Edit_Get_ExistingUser_ReturnsViewResult_WithUserViewModel()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId, Login = "User1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Phone = "+380123456789", AddressId = 1, RoleId = 1 };
            var userViewModel = new UserViewModel { UserId = userId, Login = "User1", Email = "user1@example.com", FirstName = "John", LastName = "Doe", Gender = "Male", Phone = "+380123456789", Address = "Street 1, City 1, Country 1", RoleName = "Admin" };

            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserViewModel>(user)).Returns(userViewModel);

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.Equal("User1", model.Login);
        }

        [Fact]
        public async Task Edit_Get_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 99;
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Edit(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var userViewModel = new UserViewModel
            {
                UserId = 1,
                Login = "UpdatedUser",
                Email = "updateduser@example.com",
                FirstName = "Updated",
                LastName = "User",
                Gender = "Female",
                Phone = "+380998877665",
                Address = "Updated Street, Updated City, Updated Country",
                RoleName = "Admin",
                BankCardData = "6543-2109-8765-4321"
            };

            var user = new User
            {
                UserId = 1,
                Login = "UpdatedUser",
                Email = "updateduser@example.com",
                FirstName = "Updated",
                LastName = "User",
                Gender = "Female",
                Phone = "+380998877665",
                AddressId = 4,
                RoleId = 1,
                BankCardData = "6543-2109-8765-4321"
            };

            _mockMapper.Setup(mapper => mapper.Map<User>(userViewModel)).Returns(user);
            _mockUserService.Setup(service => service.UpdateUserAsync(user)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Edit(userViewModel.UserId, userViewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockUserService.Verify(service => service.UpdateUserAsync(user), Times.Once);
        }

        [Fact]
        public async Task Edit_Post_InvalidModel_ReturnsViewResult_WithModel()
        {
            // Arrange
            var userViewModel = new UserViewModel
            {
                UserId = 1,
                // Missing required fields
            };
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Edit(userViewModel.UserId, userViewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(userViewModel, model);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_Get_ExistingUser_ReturnsViewResult_WithUserViewModel()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId, Login = "User1" };
            var userViewModel = new UserViewModel { UserId = userId, Login = "User1" };

            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserViewModel>(user)).Returns(userViewModel);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserViewModel>(viewResult.Model);
            Assert.Equal(userId, model.UserId);
            Assert.Equal("User1", model.Login);
        }

        [Fact]
        public async Task Delete_Get_NonExistingUser_ReturnsNotFoundResult()
        {
            // Arrange
            var userId = 99;
            _mockUserService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_Post_ValidUser_RedirectsToIndex()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(service => service.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteConfirmed(userId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            _mockUserService.Verify(service => service.DeleteUserAsync(userId), Times.Once);
        }

        #endregion
    }
}
