using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspShop.Models;
using System.Security.Claims;

namespace MyAspShop.Controllers
{
    public class UserController : Controller
    {
        private readonly MyShopContext _context;

        public UserController(MyShopContext context)
        {
            _context = context;
        }

        // Catalog action to display products
        public IActionResult Catalog(string? searchQuery, int? categoryId, string? sortOrder)
        {
            var productsQuery = _context.Products.AsQueryable();

            // Фильтрация по запросу поиска
            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p => p.NameProduct.Contains(searchQuery));
            }

            // Фильтрация по категории
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.ProductTypeId == categoryId);
            }

            // Сортировка
            productsQuery = sortOrder switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.PriceProduct),
                "price_desc" => productsQuery.OrderByDescending(p => p.PriceProduct),
                _ => productsQuery.OrderBy(p => p.NameProduct)  // По алфавиту
            };

            var products = productsQuery.ToList();
            var categories = _context.ProductTypes.ToList();

            // Передаем категории в ViewBag
            ViewBag.Categories = categories;

            return View(products);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == productId);
            if (product == null)
            {
                return NotFound();
            }

            // Get current user
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the item in the cart for this user
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.IdProduct == productId && ci.UserId == userId);
            if (cartItem != null)
            {
                // Increase quantity if the item is already in the cart
                cartItem.Quantity += quantity;
                cartItem.Price = cartItem.Quantity * product.PriceProduct;
            }
            else
            {
                // If the item is not in the cart, add it
                cartItem = new CartItem
                {
                    IdProduct = product.IdProduct,
                    Quantity = quantity,
                    Price = quantity * product.PriceProduct,
                    UserId = userId
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();
            return RedirectToAction("Cart", "User");
        }


        public IActionResult Cart()
        {
            var userId = User.Identity?.Name;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.IdProductNavigation) // Используем навигационное свойство
                .ToList();

            ViewBag.TotalSum = cartItems.Sum(c => c.Price); // Общая сумма корзины
            return View(cartItems); // Передаем список CartItems в представление
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int newQuantity)
        {
            var cartItem = await _context.CartItems.Include(c => c.IdProductNavigation).FirstOrDefaultAsync(c => c.Id == cartItemId);

            if (cartItem != null && newQuantity > 0)
            {
                cartItem.Quantity = newQuantity;
                cartItem.Price = cartItem.IdProductNavigation.PriceProduct * newQuantity; // Use navigation property
                await _context.SaveChangesAsync();

                return RedirectToAction("Cart", "User");
            }

            return Json(new { success = false, message = "Некорректное количество или товар не найден" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cart", "User");
        }
    }
}
