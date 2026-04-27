using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

[Route("admin/customers")]
public class AdminCustomersController : AdminBaseController
{
    public AdminCustomersController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Customers(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Customers");

        var query = Context.KhachHangs.Include(x => x.DonHangs).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaKhachHang.Contains(keyword) || x.TenKhachHang.Contains(keyword) || (x.Email ?? string.Empty).Contains(keyword) || (x.SoDienThoai ?? string.Empty).Contains(keyword));
        }

        var customers = await query.OrderBy(x => x.TenKhachHang).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? customers.FirstOrDefault(x => x.MaKhachHang == selectedId) : customers.FirstOrDefault();

        var model = new AdminCustomersViewModel
        {
            Customers = customers,
            Keyword = keyword,
            SelectedId = selected?.MaKhachHang,
            SelectedCustomer = selected,
            FormCustomer = selected is not null ? new KhachHang { MaKhachHang = selected.MaKhachHang, TenKhachHang = selected.TenKhachHang, SoDienThoai = selected.SoDienThoai, Email = selected.Email, MatKhau = selected.MatKhau } : new KhachHang()
        };

        return View("~/Views/Admin/Customers.cshtml", model);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCustomer(KhachHang customer)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        try
        {
            customer.TenKhachHang = customer.TenKhachHang?.Trim() ?? string.Empty;
            customer.Email = customer.Email?.Trim();
            customer.SoDienThoai = customer.SoDienThoai?.Trim();

            if (string.IsNullOrWhiteSpace(customer.TenKhachHang))
            {
                TempData["Error"] = "Ten khach hang khong duoc de trong.";
                return RedirectToAction(nameof(Customers), new { selectedId = customer.MaKhachHang });
            }

            var existing = !string.IsNullOrWhiteSpace(customer.MaKhachHang) ? await Context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == customer.MaKhachHang) : null;
            if (existing is null)
            {
                customer.MaKhachHang = string.IsNullOrWhiteSpace(customer.MaKhachHang) ? $"KH{DateTime.Now:MMddHHmmss}" : customer.MaKhachHang.Trim().ToUpperInvariant();
                customer.MatKhau = string.IsNullOrWhiteSpace(customer.MatKhau) ? "1234567" : customer.MatKhau;
                customer.DaXoa = false;
                Context.KhachHangs.Add(customer);
            }
            else
            {
                existing.TenKhachHang = customer.TenKhachHang;
                existing.Email = customer.Email;
                existing.SoDienThoai = customer.SoDienThoai;
                if (!string.IsNullOrWhiteSpace(customer.MatKhau)) existing.MatKhau = customer.MatKhau;
            }

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Customers), new { selectedId = customer.MaKhachHang });
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the luu khach hang. Vui long kiem tra email, so dien thoai va du lieu nhap.";
            return RedirectToAction(nameof(Customers), new { selectedId = customer.MaKhachHang });
        }
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        var customer = await Context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == id);
        if (customer is null) return RedirectToAction(nameof(Customers));

        customer.DaXoa = true;
        await Context.SaveChangesAsync();
        TempData["Success"] = "Da an khach hang khoi danh sach.";
        return RedirectToAction(nameof(Customers));
    }
}
