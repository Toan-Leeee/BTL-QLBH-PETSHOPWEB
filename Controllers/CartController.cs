using Microsoft.AspNetCore.Mvc;
using SieuPetMvc.Services;

namespace SieuPetMvc.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.BodyClass = "cart-page";
        var cart = await _cartService.GetCartAsync();
        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string id, int quantity = 1, string? returnUrl = null)
    {
        await _cartService.AddAsync(id, quantity);
        TempData["Success"] = "Da them san pham vao gio hang.";

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BuyNow(string id, int quantity = 1)
    {
        await _cartService.AddAsync(id, quantity);
        return RedirectToAction("Index", "Checkout");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string productId, int quantity)
    {
        await _cartService.UpdateAsync(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string productId)
    {
        await _cartService.RemoveAsync(productId);
        TempData["Success"] = "Da xoa san pham khoi gio hang.";
        return RedirectToAction(nameof(Index));
    }
}
