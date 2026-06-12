using HealthSync.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthSync.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var items = Cart.GetItems(HttpContext);
            ViewBag.Subtotal = Cart.GetSubtotal(HttpContext);
            ViewBag.IVA = ViewBag.Subtotal * 0.13m;
            ViewBag.Shipping = 0m; // Envío gratis
            ViewBag.Total = ViewBag.Subtotal + ViewBag.IVA;
            return View(items);
        }

        [HttpPost]
        public IActionResult Add(CartItem item)
        {
            Cart.AddItem(HttpContext, item);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, count = Cart.GetTotalItems(HttpContext) });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(string id)
        {
            Cart.RemoveItem(HttpContext, id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(string id, int quantity)
        {
            Cart.UpdateQuantity(HttpContext, id, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Clear()
        {
            Cart.Clear(HttpContext);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            if (!Cart.GetItems(HttpContext).Any())
            {
                TempData["Error"] = "Tu carrito está vacío";
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = new OrderConfirmationModel
            {
                OrderNumber = "HS-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                CustomerName = model.FullName,
                CustomerEmail = model.Email,
                Total = Cart.GetSubtotal(HttpContext) * 1.13m,
                OrderDate = DateTime.Now
            };

            Cart.Clear(HttpContext);
            return RedirectToAction("OrderConfirmation", new { orderNumber = order.OrderNumber });
        }

        public IActionResult OrderConfirmation(string orderNumber)
        {
            var model = new OrderConfirmationModel
            {
                OrderNumber = orderNumber,
                CustomerName = "Cliente",
                CustomerEmail = "",
                Total = 0,
                OrderDate = DateTime.Now
            };
            return View(model);
        }
    }
}