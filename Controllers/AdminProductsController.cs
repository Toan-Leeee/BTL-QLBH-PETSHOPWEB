using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

[Route("admin/products")]
public class AdminProductsController : AdminBaseController
{
    public AdminProductsController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Products(string? keyword, string? statusFilter, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Products");

        var categories = await Context.DanhMucs.OrderBy(x => x.TenDanhMuc).ToListAsync();
        var query = Context.SanPhams.Include(x => x.DanhMuc).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaSanPham.Contains(keyword) || x.TenSanPham.Contains(keyword) || (x.MoTa ?? string.Empty).Contains(keyword));
        }

        if (statusFilter == "active") query = query.Where(x => x.TrangThai);
        if (statusFilter == "hidden") query = query.Where(x => !x.TrangThai);

        var products = await query
            .OrderByDescending(x => x.TrangThai)
            .ThenBy(x => x.MaSanPham.StartsWith("PK"))
            .ThenBy(x => x.MaSanPham)
            .ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? products.FirstOrDefault(x => x.MaSanPham == selectedId) : null;

        var model = new AdminProductsViewModel
        {
            Products = products,
            Categories = categories,
            Keyword = keyword,
            StatusFilter = statusFilter,
            SelectedId = selectedId,
            FormProduct = selected is not null
                ? new SanPham
                {
                    MaSanPham = selected.MaSanPham,
                    MaDanhMuc = selected.MaDanhMuc,
                    TenSanPham = selected.TenSanPham,
                    GiaBan = selected.GiaBan,
                    SoLuongTonKho = selected.SoLuongTonKho,
                    MoTa = selected.MoTa,
                    HinhAnh = selected.HinhAnh,
                    TrangThai = selected.TrangThai
                }
                : new SanPham { MaDanhMuc = categories.FirstOrDefault()?.MaDanhMuc ?? string.Empty, TrangThai = true, SoLuongTonKho = 1, HinhAnh = "/images/products/default.svg" }
        };

        return View("~/Views/Admin/Products.cshtml", model);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProduct(SanPham product)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        try
        {
            product.HinhAnh = string.IsNullOrWhiteSpace(product.HinhAnh) ? "/images/products/default.svg" : product.HinhAnh.Trim();
            product.TenSanPham = product.TenSanPham?.Trim() ?? string.Empty;
            product.MoTa = product.MoTa?.Trim();

            if (string.IsNullOrWhiteSpace(product.MaDanhMuc) || string.IsNullOrWhiteSpace(product.TenSanPham))
            {
                TempData["Error"] = "Vui long nhap day du ten san pham va danh muc.";
                return RedirectToAction(nameof(Products), new { selectedId = product.MaSanPham });
            }

            if (product.GiaBan < 0 || product.SoLuongTonKho < 0)
            {
                TempData["Error"] = "Gia ban va ton kho khong duoc am.";
                return RedirectToAction(nameof(Products), new { selectedId = product.MaSanPham });
            }

            var existing = !string.IsNullOrWhiteSpace(product.MaSanPham)
                ? await Context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == product.MaSanPham)
                : null;

            if (existing is null)
            {
                var prefix = product.MaDanhMuc == "DM003" ? "PK" : "SP";
                product.MaSanPham = string.IsNullOrWhiteSpace(product.MaSanPham)
                    ? await GenerateProductCodeAsync(prefix)
                    : product.MaSanPham.Trim().ToUpperInvariant();
                product.DaXoa = false;
                Context.SanPhams.Add(product);
                TempData["Success"] = "Da them san pham moi.";
            }
            else
            {
                existing.MaDanhMuc = product.MaDanhMuc;
                existing.TenSanPham = product.TenSanPham;
                existing.GiaBan = product.GiaBan;
                existing.SoLuongTonKho = product.SoLuongTonKho;
                existing.MoTa = product.MoTa;
                existing.HinhAnh = product.HinhAnh;
                existing.TrangThai = product.TrangThai;
                TempData["Success"] = "Da cap nhat san pham.";
            }

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Products), new { selectedId = product.MaSanPham });
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the luu san pham. Vui long kiem tra ma, gia va du lieu nhap.";
            return RedirectToAction(nameof(Products), new { selectedId = product.MaSanPham });
        }
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var product = await Context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == id);
        if (product is null) return RedirectToAction(nameof(Products));

        product.TrangThai = false;

        await Context.SaveChangesAsync();
        TempData["Success"] = "Da an san pham. Ban ghi van duoc giu trong database.";
        return RedirectToAction(nameof(Products));
    }

    private async Task<string> GenerateProductCodeAsync(string prefix)
    {
        var candidate = $"{prefix}{DateTime.Now:MMddHHmmss}";
        if (!await Context.SanPhams.AnyAsync(x => x.MaSanPham == candidate))
        {
            return candidate;
        }

        for (var i = 1; i <= 99; i++)
        {
            candidate = $"{prefix}{DateTime.Now:ddHHmm}{i:00}";
            if (!await Context.SanPhams.AnyAsync(x => x.MaSanPham == candidate))
            {
                return candidate;
            }
        }

        return $"{prefix}{Guid.NewGuid():N}"[..12];
    }
}
