using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Services;

namespace SieuPetMvc.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ApplicationDbContext _context;

    public CartController(ICartService cartService, ApplicationDbContext context)
    {
        _cartService = cartService;
        _context = context;
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
        var product = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaSanPham == productId);
        if (product is not null && product.SoLuongTonKho > 0 && quantity > product.SoLuongTonKho)
        {
            TempData["Error"] = $"Sản phẩm này chỉ còn {product.SoLuongTonKho} item trong kho.";
        }

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
