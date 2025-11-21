using System.Security.Claims;
using master.core.Enums;
using master.core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Master.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManager _userManager;
        private readonly IUserRepository _users;

        public AccountController(IUserManager userManager, IUserRepository users)
        {
            _userManager = userManager;
            _users = users;
        }

        // --------------------------------------------------------
        // REGISTER
        // --------------------------------------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVm());
        }

        public class RegisterVm
        {
            public UserRole Role { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (string.IsNullOrWhiteSpace(vm.FullName) ||
                string.IsNullOrWhiteSpace(vm.Email) ||
                string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError("", "All fields are required");
                return View(vm);
            }

            var (ok, error) = await _userManager.RegisterAsync(
                                vm.FullName,
                                vm.Email,
                                vm.Password,
                                vm.Role);

            if (!ok)
            {
                ModelState.AddModelError("", error);
                return View(vm);
            }

            return RedirectToAction(nameof(Login));
        }

        // --------------------------------------------------------
        // LOGIN
        // --------------------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVm());
        }

        public class LoginVm
        {
            public UserRole Role { get; set; } = UserRole.User;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            var (ok, user, error) = await _userManager.ValidateCredentialsAsync(vm.Email, vm.Password);

            if (!ok || user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(vm);
            }

            // ROLE CHECK (User → User, Manager → Manager)
            if (user.Role != vm.Role)
            {
                ModelState.AddModelError("", "Selected role does not match your account role");
                return View(vm);
            }

            // CREATE USER CLAIMS
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(
                               claims,
                               CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // SIGN IN USER
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                });

            // REDIRECT BASED ON ROLE
            return RedirectToAction("Index", "Task");
        }

        // --------------------------------------------------------
        // LOGOUT
        // --------------------------------------------------------
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // --------------------------------------------------------
        // ACCESS DENIED
        // --------------------------------------------------------
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
