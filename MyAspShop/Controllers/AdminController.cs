using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyAspShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MyAspShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly MyShopContext _context;

        public AdminController(MyShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["Layout"] = "~/Views/Shared/AdminLayout.cshtml";
            return View();
        }

        public async Task<IActionResult> Products()
        {
            ViewData["Layout"] = "~/Views/Shared/AdminLayout.cshtml";
            var products = await _context.Products.Include(p => p.ProductType).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            ViewBag.ProductTypes = _context.ProductTypes.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Products");
            }
            ViewBag.ProductTypes = _context.ProductTypes.ToList();
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int? id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            ViewBag.ProductTypes = _context.ProductTypes.ToList();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(product.IdProduct);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Обновление свойств
                existingProduct.NameProduct = product.NameProduct;
                existingProduct.PriceProduct = product.PriceProduct;
                existingProduct.ProductTypeId = product.ProductTypeId;
                existingProduct.DescriptionProduct = product.DescriptionProduct;

                // Обновление с учетом версии
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Products");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Обновление не удалось. Продукт был изменен другим пользователем.");
                }
            }

            ViewBag.ProductTypes = _context.ProductTypes.ToList();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Products));
        }

        // CRUD действия для ProductType

        public async Task<IActionResult> ProductTypes()
        {
            ViewData["Layout"] = "~/Views/Shared/AdminLayout.cshtml";
            var productTypes = await _context.ProductTypes.ToListAsync();
            return View(productTypes);
        }

        [HttpGet]
        public IActionResult CreateProductType()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductType(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                _context.ProductTypes.Add(productType);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProductTypes");
            }
            return View(productType);
        }

        [HttpGet]
        public async Task<IActionResult> EditProductType(int? id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
                return NotFound();
            return View(productType);
        }

        [HttpPost]
        public async Task<IActionResult> EditProductType(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                var existingProductType = await _context.ProductTypes.FindAsync(productType.IdProductType);
                if (existingProductType == null)
                {
                    return NotFound();
                }

                existingProductType.NameType = productType.NameType;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ProductTypes");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Обновление не удалось. Тип продукта был изменен другим пользователем.");
                }
            }
            return View(productType);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await _context.ProductTypes
                .Include(pt => pt.Products)
                .FirstOrDefaultAsync(pt => pt.IdProductType == id);

            if (productType != null)
            {
                if (productType.Products.Any()) // Проверяем наличие связанных продуктов
                {
                    ModelState.AddModelError("", "Невозможно удалить тип продукта, так как существуют связанные продукты.");
                    return RedirectToAction(nameof(ProductTypes)); // Или вы можете отобразить сообщение об ошибке
                }

                _context.ProductTypes.Remove(productType);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProductTypes));
        }

        public async Task<IActionResult> Reviews()
        {
            ViewData["Layout"] = "~/Views/Shared/AdminLayout.cshtml";
            var reviews = await _context.Reviews.Include(r => r.Product).Include(r => r.Users).ToListAsync();
            return View(reviews);
        }

        [HttpGet]
        [HttpGet]
        public IActionResult CreateReview()
        {
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(Review review)
        {
            if (ModelState.IsValid)
            {
                // Проверим, существует ли выбранный пользователь и продукт
                var user = await _context.Users.FindAsync(review.UsersId);
                var product = await _context.Products.FindAsync(review.ProductId);

                if (user == null || product == null)
                {
                    ModelState.AddModelError("", "Пользователь или продукт не существует.");
                    ViewBag.Products = _context.Products.ToList();
                    ViewBag.Users = _context.Users.ToList();
                    return View(review);
                }

                // Добавляем отзыв в базу данных
                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Перенаправляем на страницу с отзывами
                return RedirectToAction("Reviews");
            }

            // Если модель не валидна, выводим ошибки и возвращаем представление
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Users = _context.Users.ToList();
            return View(review);
        }



        [HttpGet]
        public async Task<IActionResult> EditReview(int? id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Users = _context.Users.ToList();
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost]
        public async Task<IActionResult> EditReview(Review review)
        {
            if (ModelState.IsValid)
            {
                var existingReview = await _context.Reviews.FindAsync(review.IdReview);
                if (existingReview == null)
                {
                    return NotFound();
                }

                existingReview.Rating = review.Rating;
                existingReview.CommentReview = review.CommentReview;
                existingReview.UsersId = review.UsersId;
                existingReview.ProductId = review.ProductId;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Reviews");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Обновление не удалось. Отзыв был изменен другим пользователем.");
                }
            }

            ViewBag.Products = _context.Products.ToList();
            ViewBag.Users = _context.Users.ToList();
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Reviews));
        }

        public async Task<IActionResult> Users()
        {
            ViewData["Layout"] = "~/Views/Shared/AdminLayout.cshtml";
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Users");
            }
            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(user.IdUser);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.LoginUser = user.LoginUser;
                existingUser.PasswordUser = user.PasswordUser;
                existingUser.Email = user.Email;
                existingUser.RoleId = user.RoleId;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Users");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Обновление не удалось. Пользователь был изменен другим администратором.");
                }
            }

            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Users");
        }

        public async Task<IActionResult> Roles()
        {
            ViewData["Layout"] = "~/Views/Shared/AdminLayout.cshtml";
            var roles = await _context.Roles.ToListAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role)
        {
            if (ModelState.IsValid)
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction("Roles");
            }
            return View(role);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(int? id)
        {
            if (id == null)
                return NotFound();

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(Role role)
        {
            if (ModelState.IsValid)
            {
                var existingRole = await _context.Roles.FindAsync(role.IdRole);
                if (existingRole == null)
                {
                    return NotFound();
                }

                existingRole.NameRole = role.NameRole;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Roles");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Обновление не удалось. Роль была изменена другим администратором.");
                }
                catch (Exception ex) // Общая обработка исключений
                {
                    ModelState.AddModelError("", $"Произошла ошибка: {ex.Message}");
                }
            }

            // Если модель не валидна или произошла ошибка, возвращаем представление с текущими данными
            return View(role);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Roles");
        }

    }
}