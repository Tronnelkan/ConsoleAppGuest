// CourseProject.Web/Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using CourseProject.BLL.Services;
using CourseProject.Web.ViewModels;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseProject.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IAddressService addressService, IRoleService roleService, IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            Console.WriteLine("UsersController: Залежності впроваджено.");
        }


        // GET: Users
        [Authorize()]
        public async Task<IActionResult> Index()
        {
            Console.WriteLine("Початок дії Index.");

            // Отримання користувачів
            var users = await _userService.GetAllUsersAsync();
            var userViewModels = _mapper.Map<IEnumerable<UserViewModel>>(users);

            // Отримання всіх ролей та адрес
            var roles = await _roleService.GetAllRolesAsync();
            var addresses = await _addressService.GetAllAddressesAsync();

            foreach (var user in users)
            {
                var viewModel = userViewModels.FirstOrDefault(u => u.UserId == user.UserId);

                if (viewModel != null)
                {
                    // Призначення RoleName
                    var role = roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                    viewModel.RoleName = role?.RoleName ?? "Роль не призначена";

                    // Призначення Address
                    var address = addresses.FirstOrDefault(a => a.AddressId == user.AddressId);
                    viewModel.Address = address != null
                        ? $"{address.Street}, {address.City}, {address.Country}"
                        : "Адреса не призначена";
                }
            }

            Console.WriteLine($"Отримано {userViewModels.Count()} користувачів з ролями та адресами.");
            return View(userViewModels);
        }



        // GET: Users/Details/5
        [Authorize()]
        public async Task<IActionResult> Details(int id)
        {
            Console.WriteLine($"Початок дії Details для користувача з ID: {id}");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine($"Користувача з ID {id} не знайдено.");
                return NotFound();
            }

            var userViewModel = _mapper.Map<UserViewModel>(user);

            // Отримання ролі
            var role = await _roleService.GetRoleByIdAsync(user.RoleId);
            userViewModel.RoleName = role?.RoleName ?? "Роль не призначена";

            // Отримання адреси
            var address = await _addressService.GetAddressByIdAsync(user.AddressId);
            if (address != null)
            {
                userViewModel.Address = $"{address.Street}, {address.City}, {address.Country}";
            }
            else
            {
                userViewModel.Address = "Адреса не призначена";
            }

            Console.WriteLine($"Деталі користувача: {userViewModel.Login}, роль: {userViewModel.RoleName}, адреса: {userViewModel.Address}.");
            return View(userViewModel);
        }


        // GET: Users/Create
        [Authorize()]
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
                // Ви можете встановити RoleId, AddressId за потребою
                await _userService.AddUserAsync(user); // Додайте відповідний метод у IUserService
                TempData["SuccessMessage"] = "Користувача успішно створено.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Users/Edit/5
        [Authorize()]
        public async Task<IActionResult> Edit(int id)
        {
            Console.WriteLine($"Початок дії Edit (GET) для користувача з ID: {id}");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                Console.WriteLine($"Користувача з ID {id} не знайдено.");
                return NotFound();
            }

            var userViewModel = _mapper.Map<UserViewModel>(user);
            Console.WriteLine($"Користувач {user.Login} замаплений до UserViewModel.");

            // Отримати всі доступні ролі
            var roles = await _roleService.GetAllRolesAsync();
            if (roles == null || !roles.Any())
            {
                Console.WriteLine("Немає доступних ролей у базі даних.");
                ModelState.AddModelError("", "Немає доступних ролей. Зверніться до адміністратора.");
                return View(userViewModel);
            }

            userViewModel.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.RoleName,
                Text = r.RoleName
            });

            Console.WriteLine($"Заповнено {userViewModel.Roles.Count()} ролей у ViewModel.");

            return View(userViewModel);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Edit(int id, UserViewModel model)
        {
            if (id != model.UserId)
            {
                Console.WriteLine($"ID користувача з URL ({id}) не співпадає з ID у моделі ({model.UserId}).");
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Отримати існуючого користувача
                    var existingUser = await _userService.GetUserByIdAsync(id);
                    if (existingUser == null)
                    {
                        Console.WriteLine($"Користувача з ID {id} не знайдено.");
                        return NotFound();
                    }

                    // Оновити властивості користувача
                    existingUser.Login = model.Login;
                    existingUser.Email = model.Email;
                    existingUser.FirstName = model.FirstName;
                    existingUser.LastName = model.LastName;
                    existingUser.Gender = model.Gender;
                    existingUser.Phone = model.Phone;
                    existingUser.BankCardData = model.BankCardData;

                    // Обробка адреси
                    if (!string.IsNullOrWhiteSpace(model.Address))
                    {
                        var parts = model.Address.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 3)
                        {
                            var street = parts[0].Trim();
                            var city = parts[1].Trim();
                            var country = parts[2].Trim();

                            var newAddress = new Address
                            {
                                Street = street,
                                City = city,
                                Country = country
                            };

                            await _addressService.AddAddressAsync(newAddress);
                            Console.WriteLine($"Додано нову адресу: {newAddress.Street}, {newAddress.City}, {newAddress.Country}");

                            existingUser.AddressId = newAddress.AddressId;
                            existingUser.AddressEntity = newAddress;
                        }
                        else
                        {
                            Console.WriteLine("Невірний формат адреси.");
                            ModelState.AddModelError("Address", "Адреса повинна містити вулицю, місто та країну, розділені комами.");

                            // Повторно завантажити ролі для відображення у випадаючому списку
                            var roles = await _roleService.GetAllRolesAsync();
                            model.Roles = roles.Select(r => new SelectListItem
                            {
                                Value = r.RoleName,
                                Text = r.RoleName
                            });
                            Console.WriteLine($"Заповнено {model.Roles.Count()} ролей у ViewModel.");

                            return View(model);
                        }
                    }

                    // Оновити роль користувача
                    var role = await _roleService.GetRoleByNameAsync(model.RoleName);
                    if (role == null)
                    {
                        Console.WriteLine($"Роль '{model.RoleName}' не знайдена.");
                        ModelState.AddModelError("RoleName", "Обрана роль не існує.");

                        // Повторно завантажити ролі для відображення у випадаючому списку
                        var roles = await _roleService.GetAllRolesAsync();
                        model.Roles = roles.Select(r => new SelectListItem
                        {
                            Value = r.RoleName,
                            Text = r.RoleName
                        });
                        Console.WriteLine($"Заповнено {model.Roles.Count()} ролей у ViewModel.");

                        return View(model);
                    }
                    existingUser.RoleId = role.RoleId;
                    existingUser.Role = role;
                    Console.WriteLine($"Присвоєно користувачу роль: {role.RoleName}");

                    // Оновити користувача через сервіс
                    await _userService.UpdateUserAsync(existingUser);
                    Console.WriteLine("Користувача успішно оновлено.");

                    TempData["SuccessMessage"] = "Користувача успішно оновлено.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex} Помилка при оновленні користувача з ID {id}");
                    ModelState.AddModelError("", $"Помилка при оновленні користувача: {ex.Message}");

                    // Повторно завантажити ролі для відображення у випадаючому списку
                    var roles = await _roleService.GetAllRolesAsync();
                    model.Roles = roles.Select(r => new SelectListItem
                    {
                        Value = r.RoleName,
                        Text = r.RoleName
                    });
                    Console.WriteLine($"Заповнено {model.Roles.Count()} ролей у ViewModel.");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine("ModelState не валідний.");

                // Логування помилок ModelState
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }

                // Повторно завантажити ролі для відображення у випадаючому списку
                var rolesReload = await _roleService.GetAllRolesAsync();
                if (rolesReload != null && rolesReload.Any())
                {
                    model.Roles = rolesReload.Select(r => new SelectListItem
                    {
                        Value = r.RoleName,
                        Text = r.RoleName
                    });
                    Console.WriteLine($"Заповнено {model.Roles.Count()} ролей у ViewModel.");
                }
                else
                {
                    Console.WriteLine("Немає доступних ролей у базі даних.");
                    ModelState.AddModelError("", "Немає доступних ролей. Зверніться до адміністратора.");
                }
            }
            return View(model);
        }



        // GET: Users/Delete/5
        [Authorize()]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var userViewModel = _mapper.Map<UserViewModel>(user);
            return View(userViewModel);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["SuccessMessage"] = "Користувача успішно видалено.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при видаленні користувача з ID {id}: {ex.Message}");
                ModelState.AddModelError("", $"Помилка при видаленні користувача: {ex.Message}");
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();
                var userViewModel = _mapper.Map<UserViewModel>(user);
                return View(userViewModel);
            }
        }
    }
}
