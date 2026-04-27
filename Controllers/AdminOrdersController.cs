using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

[Route("admin/orders")]
public class AdminOrdersController : AdminBaseController
{
    public AdminOrdersController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Orders(string? keyword, string? status, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Orders");

        var query = Context.DonHangs
            .Include(x => x.KhachHang)
            .Include(x => x.NhanVien)
            .Include(x => x.ChiTietDonHangs)
                .ThenInclude(x => x.SanPham)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaDonHang.Contains(keyword)
                || (x.KhachHang != null && x.KhachHang.TenKhachHang.Contains(keyword))
                || (x.NhanVien != null && x.NhanVien.HoTen.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.TrangThai == status);
        }

        var orders = await query.OrderByDescending(x => x.NgayTao).ToListAsync();
        var selectedOrder = !string.IsNullOrWhiteSpace(selectedId) ? orders.FirstOrDefault(x => x.MaDonHang == selectedId) : orders.FirstOrDefault();

        var model = new AdminOrdersViewModel
        {
            Orders = orders,
            Keyword = keyword,
            Status = status,
            SelectedId = selectedOrder?.MaDonHang,
            SelectedOrder = selectedOrder
        };

        return View("~/Views/Admin/Orders.cshtml", model);
    }

    [HttpPost("update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrder(string id, string status)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        try
        {
            var order = await Context.DonHangs.FirstOrDefaultAsync(x => x.MaDonHang == id);
            if (order is null) return RedirectToAction(nameof(Orders));
            order.TrangThai = status;
            order.MaNhanVien ??= HttpContext.Session.GetString("EmployeeId");
            await Context.SaveChangesAsync();
            TempData["Success"] = "Da cap nhat don hang.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the cap nhat don hang luc nay.";
        }

        return RedirectToAction(nameof(Orders), new { selectedId = id });
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteOrder(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        try
        {
            var order = await Context.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .FirstOrDefaultAsync(x => x.MaDonHang == id);

            if (order is null)
            {
                return RedirectToAction(nameof(Orders));
            }

            var canRestoreStock = order.TrangThai is "Cho xac nhan" or "Cho thanh toan";
            if (canRestoreStock)
            {
                var productIds = order.ChiTietDonHangs.Select(x => x.MaSanPham).Distinct().ToList();
                var products = await Context.SanPhams
                    .Where(x => productIds.Contains(x.MaSanPham))
                    .ToDictionaryAsync(x => x.MaSanPham, x => x);

                foreach (var detail in order.ChiTietDonHangs)
                {
                    if (!products.TryGetValue(detail.MaSanPham, out var product))
                    {
                        continue;
                    }

                    product.SoLuongTonKho += detail.SoLuong;
                    if (product.SoLuongTonKho > 0)
                    {
                        product.TrangThai = true;
                    }
                }
            }

            order.DaXoa = true;
            order.TrangThai = "Da huy";
            order.MaNhanVien ??= HttpContext.Session.GetString("EmployeeId");

            await Context.SaveChangesAsync();
            TempData["Success"] = canRestoreStock
                ? "Da an don hang va hoan lai so luong ton kho."
                : "Da an don hang khoi danh sach.";
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the xoa don hang luc nay.";
        }

        return RedirectToAction(nameof(Orders));
    }
}
