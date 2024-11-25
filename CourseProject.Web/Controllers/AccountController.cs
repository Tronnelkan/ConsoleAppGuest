// CourseProject.Web/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using CourseProject.Web.ViewModels;
using CourseProject.BLL.Services;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CourseProject.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, IMapper mapper, ILogger<AccountController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = _mapper.Map<User>(model);
                    await _userService.RegisterUserAsync(user, model.Password);
                    TempData["SuccessMessage"] = "Реєстрація успішна! Будь ласка, увійдіть.";
                    return RedirectToAction(nameof(Login));
                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Помилка під час реєстрації користувача.");
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Неочікувана помилка під час реєстрації користувача.");
                    ModelState.AddModelError("", "Сталася помилка під час реєстрації. Спробуйте пізніше.");
                }
            }
            // Якщо модель не валідна або сталася помилка, повертаємо форму з введеними даними
            return View(model);
        }


        // GET: Account/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            _logger.LogInformation("Ініціалізація представлення логіну.");
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isAuthenticated = await _userService.AuthenticateAsync(model.Login, model.Password);

                if (isAuthenticated)
                {
                    // Створіть ClaimsIdentity
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Login),
                new Claim(ClaimTypes.Role, "User") // Ви можете динамічно визначати роль
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Створіть Cookie
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe, // "Запам’ятати мене"
                            ExpiresUtc = DateTime.UtcNow.AddDays(30) // Наприклад, на 30 днів
                        });

                    TempData["SuccessMessage"] = "Вхід виконано успішно!";
                    return RedirectToAction("Index", "Users");
                }

                ModelState.AddModelError("", "Невірний логін або пароль.");
            }

            return View(model);
        }




        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Ви успішно вийшли.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ForgotPassword
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isReset = await _userService.ResetPasswordAsync(model.Email, model.RecoveryKeyword, model.NewPassword);
                if (isReset)
                {
                    TempData["SuccessMessage"] = "Пароль успішно скинуто! Ви можете увійти з новим паролем.";
                    return RedirectToAction(nameof(Login));
                }
                ModelState.AddModelError("", "Не вдалося скинути пароль. Перевірте Email та Recovery Keyword.");
            }
            return View(model);
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Додайте інші дії за потребою
    }
}
