using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.Services;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;

    public CheckoutController(ApplicationDbContext context, ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.BodyClass = "checkout-page";
        var cart = await _cartService.GetCartAsync();
        if (!cart.Items.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var model = new CheckoutViewModel
        {
            Cart = cart
        };

        var customerId = HttpContext.Session.GetString("CustomerId");
        if (!string.IsNullOrWhiteSpace(customerId))
        {
            var customer = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == customerId);
            if (customer is not null)
            {
                model.FullName = customer.TenKhachHang;
                model.Phone = customer.SoDienThoai ?? string.Empty;
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        ViewBag.BodyClass = "checkout-page";
        model.Cart = await _cartService.GetCartAsync();
        if (!model.Cart.Items.Any())
        {
            ModelState.AddModelError(string.Empty, "Gio hang dang trong.");
        }

        var productIds = model.Cart.Items.Select(x => x.ProductId).ToList();
        var products = await _context.SanPhams.Where(x => productIds.Contains(x.MaSanPham)).ToDictionaryAsync(x => x.MaSanPham, x => x);

        foreach (var item in model.Cart.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product) || !product.TrangThai)
            {
                ModelState.AddModelError(string.Empty, $"San pham {item.Name} hien khong con kha dung.");
                continue;
            }

            if (product.SoLuongTonKho < item.Quantity)
            {
                ModelState.AddModelError(string.Empty, $"San pham {item.Name} chi con {product.SoLuongTonKho} trong kho.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var customerId = HttpContext.Session.GetString("CustomerId");
        var orderId = $"DH{DateTime.Now:yyMMddHHmmss}";

        var order = new DonHang
        {
            MaDonHang = orderId,
            MaKhachHang = customerId,
            MaNhanVien = "NV001",
            NgayTao = DateTime.Now,
            TrangThai = "Cho xac nhan",
            TongTien = model.Cart.GrandTotal
        };

        _context.DonHangs.Add(order);
        var index = 1;
        foreach (var item in model.Cart.Items)
        {
            var product = products[item.ProductId];
            product.SoLuongTonKho -= item.Quantity;
            if (product.SoLuongTonKho <= 0)
            {
                product.SoLuongTonKho = 0;
                product.TrangThai = false;
            }

            _context.ChiTietDonHangs.Add(new ChiTietDonHang
            {
                MaChiTietDonHang = $"CT{DateTime.Now:HHmmss}{index:00}",
                MaDonHang = orderId,
                MaSanPham = item.ProductId,
                SoLuong = item.Quantity,
                DonGiaBan = item.Price,
                ThanhTien = item.LineTotal
            });
            index++;
        }

        await _context.SaveChangesAsync();
        await _cartService.ClearAsync();

        return RedirectToAction(nameof(Success), new { id = orderId });
    }

    public async Task<IActionResult> Success(string id)
    {
        ViewBag.BodyClass = "success-page";

        var order = await _context.DonHangs
            .Include(x => x.ChiTietDonHangs)
            .ThenInclude(x => x.SanPham)
            .FirstOrDefaultAsync(x => x.MaDonHang == id);

        if (order is null)
        {
            return RedirectToAction("Index", "Home");
        }

        var vm = new OrderSuccessViewModel
        {
            Order = order,
            Items = order.ChiTietDonHangs.Select(x => new CartItemViewModel
            {
                ProductId = x.MaSanPham,
                Name = x.SanPham?.TenSanPham ?? string.Empty,
                ImageUrl = x.SanPham?.HinhAnh ?? "/images/products/default.svg",
                Subtitle = x.SanPham?.MoTa ?? string.Empty,
                Price = x.DonGiaBan,
                Quantity = x.SoLuong,
                Label = x.SanPham?.MaDanhMuc == "DM003" ? "Phu kien" : "Thu cung"
            }).ToList()
        };

        return View(vm);
    }
}
