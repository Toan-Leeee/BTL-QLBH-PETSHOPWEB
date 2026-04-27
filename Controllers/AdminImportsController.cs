using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

[Route("admin/imports")]
public class AdminImportsController : AdminBaseController
{
    public AdminImportsController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Imports(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Imports");

        var query = Context.PhieuNhaps
            .Include(x => x.NhanVien)
            .Include(x => x.ChiTietPhieuNhaps)
                .ThenInclude(x => x.SanPham)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaPhieuNhap.Contains(keyword) || (x.NhaCungCap ?? string.Empty).Contains(keyword) || (x.NhanVien != null && x.NhanVien.HoTen.Contains(keyword)));
        }

        var receipts = await query.OrderByDescending(x => x.NgayNhap).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? receipts.FirstOrDefault(x => x.MaPhieuNhap == selectedId) : receipts.FirstOrDefault();

        var model = new AdminImportsViewModel
        {
            Receipts = receipts,
            Products = await Context.SanPhams.OrderBy(x => x.TenSanPham).ToListAsync(),
            Keyword = keyword,
            SelectedId = selected?.MaPhieuNhap,
            SelectedReceipt = selected,
            FormReceipt = new PhieuNhap { NgayNhap = DateTime.Now }
        };

        return View("~/Views/Admin/Imports.cshtml", model);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveImport(PhieuNhap receipt, List<string>? productIds, List<int>? quantities, List<decimal>? importPrices)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        try
        {
            productIds ??= new List<string>();
            quantities ??= new List<int>();
            importPrices ??= new List<decimal>();

            var validLines = productIds.Select((productId, index) => new
            {
                ProductId = productId,
                Quantity = index < quantities.Count ? quantities[index] : 0,
                Price = index < importPrices.Count ? importPrices[index] : 0m
            }).Where(x => !string.IsNullOrWhiteSpace(x.ProductId) && x.Quantity > 0 && x.Price >= 0).ToList();

            if (!validLines.Any())
            {
                TempData["Error"] = "Phieu nhap phai co it nhat mot dong hop le.";
                return RedirectToAction(nameof(Imports));
            }

            receipt.NhaCungCap = receipt.NhaCungCap?.Trim();
            if (string.IsNullOrWhiteSpace(receipt.NhaCungCap))
            {
                TempData["Error"] = "Vui long nhap nha cung cap.";
                return RedirectToAction(nameof(Imports));
            }

            receipt.MaPhieuNhap = string.IsNullOrWhiteSpace(receipt.MaPhieuNhap) ? $"PN{DateTime.Now:MMddHHmmss}" : receipt.MaPhieuNhap.Trim().ToUpperInvariant();
            receipt.NgayNhap = receipt.NgayNhap == default ? DateTime.Now : receipt.NgayNhap;
            receipt.MaNhanVien = HttpContext.Session.GetString("EmployeeId") ?? "NV001";
            receipt.DaXoa = false;
            Context.PhieuNhaps.Add(receipt);

            foreach (var line in validLines)
            {
                var product = await Context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == line.ProductId);
                if (product is null) continue;
                product.SoLuongTonKho += line.Quantity;
                if (product.SoLuongTonKho > 0) product.TrangThai = true;

                Context.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                {
                    MaPhieuNhap = receipt.MaPhieuNhap,
                    MaSanPham = line.ProductId,
                    SoLuongNhap = line.Quantity,
                    GiaNhap = line.Price
                });
            }

            await Context.SaveChangesAsync();
            TempData["Success"] = "Da luu phieu nhap va cap nhat ton kho.";
            return RedirectToAction(nameof(Imports), new { selectedId = receipt.MaPhieuNhap });
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the luu phieu nhap luc nay.";
            return RedirectToAction(nameof(Imports));
        }
    }
}
