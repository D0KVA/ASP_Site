using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MyAspShop.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;

namespace MyAspShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly MyShopContext _context;

        public HomeController(ILogger<HomeController> logger, MyShopContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorMessage = "Введите корректные данные для входа.";
                return View();
            }

            string hashedPassword = HashPassword(password);
            var user = _context.Users
                               .Include(u => u.Role)
                               .FirstOrDefault(u => u.Email == email && u.PasswordUser == hashedPassword);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.LoginUser),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.NameRole ?? "User")
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                if (user.Role?.NameRole == "Admin")
                {
                    return RedirectToAction("Products", "Admin");
                }

                return RedirectToAction("Catalog", "User");
            }

            ViewBag.ErrorMessage = "Неверные учетные данные";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(string fullname, string email, string password)
        {

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.ErrorMessage = "Пользователь с таким email уже существует";
                return View();
            }

            string hashedPassword = HashPassword(password);

            var user = new User
            {
                LoginUser = fullname,
                Email = email,
                PasswordUser = hashedPassword,
                RoleId = 2
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(nameof(password), "Пароль не может быть пустым.");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult DeleteCookie()
        {
            if (Request.Cookies.ContainsKey("Name"))
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    Path = "/"
                };

                Response.Cookies.Append("Name", "", cookieOptions);

                TempData["Message"] = "Куки удалены.";
            }
            else
            {
                TempData["Message"] = "Куки не найдены.";
            }

            return RedirectToAction("Index");
        }

    }
}
