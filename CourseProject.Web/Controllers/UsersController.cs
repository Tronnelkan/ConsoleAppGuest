using AutoMapper;
using CourseProject.BLL.Services;
using CourseProject.Web.Models;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseProject.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;
        private readonly IAddressService _addressService;

        // Визначте дефолтні значення для RoleId та AddressId
        private const int DefaultRoleId = 1; // "Guest"
        private const int DefaultAddressId = 1; // Стандартна адреса

        public UsersController(IUserService userService, IMapper mapper, IRoleService roleService, IAddressService addressService)
        {
            _userService = userService;
            _mapper = mapper;
            _roleService = roleService;
            _addressService = addressService;
        }

        // GET: Users
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var model = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return View(model);
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = _mapper.Map<UserViewModel>(user);
            return View(model);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);
                user.RoleId = DefaultRoleId;
                user.AddressId = DefaultAddressId;
                user.RecoveryKeyword = "defaultRecovery"; // Ви можете змінити це значення або зробити поле вводу

                try
                {
                    await _userService.RegisterUserAsync(user, model.Password);
                    return RedirectToAction(nameof(Index));
                }
                catch (System.ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = _mapper.Map<UserViewModel>(user);
            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(model.UserId);
                if (user == null)
                    return NotFound();

                // Оновлюємо властивості
                user.Login = model.Login;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Gender = model.Gender;
                user.Email = model.Email;
                user.Phone = model.Phone;
                user.BankCardData = model.BankCardData;

                if (!string.IsNullOrEmpty(model.Password))
                {
                    // Оновлюємо пароль, якщо він введений
                    user.PasswordHash = ComputeHash(model.Password);
                }

                try
                {
                    await _userService.UpdateUserAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError(string.Empty, "Не вдалося оновити користувача.");
                }
            }

            return View(model);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var model = _mapper.Map<UserViewModel>(user);
            return View(model);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }

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
    }
}
