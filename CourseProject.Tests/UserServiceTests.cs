using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

// Замініть ці простори імен на відповідні у вашому проекті
using CourseProject.DAL.Repositories;
using Domain.Models;
using CourseProject.BLL.Services;

namespace CourseProject.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRoleRepository> _roleRepositoryMock;
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            // Ініціалізація мок-об'єктів
            _userRepositoryMock = new Mock<IUserRepository>();
            _roleRepositoryMock = new Mock<IRoleRepository>();
            _addressRepositoryMock = new Mock<IAddressRepository>();

            // Створення інстансу UserService з мокованими репозиторіями
            _userService = new UserService(_userRepositoryMock.Object, _roleRepositoryMock.Object, _addressRepositoryMock.Object);
        }

        /// <summary>
        /// Вспомогательный метод для хэширования пароля, аналогичный UserService.ComputeHash
        /// </summary>
        private string ComputeHash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                var builder = new System.Text.StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        [Fact]
        public async Task ResetPasswordAsync_Successful()
        {
            // Arrange
            var email = "test@example.com";
            var recoveryKeyword = "keyword123";
            var newPassword = "newPassword";
            var hashedNewPassword = ComputeHash(newPassword);
            var oldHashedPassword = ComputeHash("oldPassword");

            var user = new User
            {
                UserId = 1,
                Email = email,
                RecoveryKeyword = recoveryKeyword,
                PasswordHash = oldHashedPassword
            };

            // Настройка мока для возвращения пользователя по email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email))
                               .ReturnsAsync(user);

            // Настройка мока для обновления пользователя
            _userRepositoryMock.Setup(repo => repo.Update(user))
                               .Callback<User>(u => u.PasswordHash = hashedNewPassword);

            // Настройка мока для сохранения изменений
            _userRepositoryMock.Setup(repo => repo.SaveChangesAsync())
                               .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.ResetPasswordAsync(email, recoveryKeyword, newPassword);

            // Assert
            Assert.True(result);
            Assert.Equal(hashedNewPassword, user.PasswordHash);
            _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
            _userRepositoryMock.Verify(repo => repo.Update(user), Times.Once);
            _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async Task GetAllUsersAsync_ReturnsUserList()
        {
            // Arrange: создаём фейковый список пользователей
            var users = new List<User>
            {
                new User { UserId = 1, Login = "user1", Email = "user1@example.com", RecoveryKeyword = "kw1" },
                new User { UserId = 2, Login = "user2", Email = "user2@example.com", RecoveryKeyword = "kw2" }
            };

            // Настройка мока: возвращаем список пользователей из репозитория
            _userRepositoryMock.Setup(repo => repo.GetAllAsync())
                               .ReturnsAsync(users); // Перевірте, що `users` має правильний тип (List<User> або IEnumerable<User>)

            // Act: вызываем метод сервиса
            var result = await _userService.GetAllUsersAsync();

            // Assert: проверяем результат
            Assert.NotNull(result); // Проверяем, что результат не null

            // Преобразуем результат в список для доступа к Count
            var resultList = result.ToList();

            // Проверяем количество элементов
            Assert.Equal(users.Count, resultList.Count); // Исправлено на resultList.Count

            // Сравниваем списки
            for (int i = 0; i < users.Count; i++)
            {
                Assert.Equal(users[i].UserId, resultList[i].UserId);
                Assert.Equal(users[i].Login, resultList[i].Login);
                Assert.Equal(users[i].Email, resultList[i].Email);
            }

            // Проверяем, что метод репозитория вызывался один раз
            _userRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ResetPasswordAsync_InvalidEmail_ReturnsFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var recoveryKeyword = "keyword123";
            var newPassword = "newPassword";

            // Настройка мока для возвращения null при неверном email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _userService.ResetPasswordAsync(email, recoveryKeyword, newPassword);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ResetPasswordAsync_InvalidRecoveryKeyword_ReturnsFalse()
        {
            // Arrange
            var email = "test@example.com";
            var recoveryKeyword = "wrongKeyword";
            var newPassword = "newPassword";

            var user = new User
            {
                UserId = 1,
                Email = email,
                RecoveryKeyword = "keyword123",
                PasswordHash = ComputeHash("oldPassword")
            };

            // Настройка мока для возвращения пользователя по email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email))
                               .ReturnsAsync(user);

            // Act
            var result = await _userService.ResetPasswordAsync(email, recoveryKeyword, newPassword);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_Successful()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                Login = "originalLogin",
                Email = "original@example.com",
                RecoveryKeyword = "keyword123"
            };

            var updatedUser = new User
            {
                UserId = userId,
                Login = "updatedLogin",
                Email = "updated@example.com",
                RecoveryKeyword = "keyword123" // Має залишитись незмінним
            };

            // Настройка мока для возвращения существующего пользователя по ID
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                               .ReturnsAsync(existingUser);

            // Настройка мока для обновления пользователя
            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(existingUser))
                               .Returns(Task.CompletedTask);

            // Act
            await _userService.UpdateUserAsync(updatedUser);

            // Assert
            Assert.Equal("updatedLogin", existingUser.Login);
            Assert.Equal("updated@example.com", existingUser.Email);
            Assert.Equal("keyword123", existingUser.RecoveryKeyword); // Має залишитись незмінним
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var updatedUser = new User
            {
                UserId = userId,
                Login = "updatedLogin",
                Email = "updated@example.com",
                RecoveryKeyword = "keyword123"
            };

            // Настройка мока для возвращения null при неверном ID
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                               .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.UpdateUserAsync(updatedUser));

            Assert.Equal("Користувач не знайдений.", exception.Message);
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { UserId = 1, Login = "user1", Email = email, RecoveryKeyword = "kw1" };

            // Настройка мока для возвращения пользователя по email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email))
                               .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Настройка мока для возвращения null при неверном email
            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(email))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(result);
            _userRepositoryMock.Verify(repo => repo.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsTrue()
        {
            // Arrange
            var login = "user1";
            var password = "password123";
            var hashedPassword = ComputeHash(password); // Правильный хэш пароля

            var user = new User
            {
                UserId = 1,
                Login = login,
                PasswordHash = hashedPassword, // Устанавливаем хэш пароля
                Email = "user1@example.com",
                RecoveryKeyword = "kw1"
            };

            // Настройка мока для возвращения пользователя по логину
            _userRepositoryMock.Setup(repo => repo.GetByLoginAsync(login))
                               .ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync(login, password);

            // Assert
            Assert.True(result);
            _userRepositoryMock.Verify(repo => repo.GetByLoginAsync(login), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var login = "user1";
            var password = "wrongPassword";
            var correctHashedPassword = ComputeHash("password123"); // Хэш правильного пароля

            var user = new User
            {
                UserId = 1,
                Login = login,
                PasswordHash = correctHashedPassword, // Устанавливаем хэш правильного пароля
                Email = "user1@example.com",
                RecoveryKeyword = "kw1"
            };

            // Настройка мока для возвращения пользователя по логину
            _userRepositoryMock.Setup(repo => repo.GetByLoginAsync(login))
                               .ReturnsAsync(user);

            // Act
            var result = await _userService.AuthenticateAsync(login, password);

            // Assert
            Assert.False(result);
            _userRepositoryMock.Verify(repo => repo.GetByLoginAsync(login), Times.Once);
        }
    }
}
