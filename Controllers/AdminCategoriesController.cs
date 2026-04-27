using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

[Route("admin/categories")]
public class AdminCategoriesController : AdminBaseController
{
    public AdminCategoriesController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Categories(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Categories");

        var query = Context.DanhMucs.Include(x => x.SanPhams).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaDanhMuc.Contains(keyword) || x.TenDanhMuc.Contains(keyword) || (x.MoTa ?? string.Empty).Contains(keyword));
        }

        var categories = await query.OrderBy(x => x.TenDanhMuc).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? categories.FirstOrDefault(x => x.MaDanhMuc == selectedId) : categories.FirstOrDefault();

        var model = new AdminCategoriesViewModel
        {
            Categories = categories,
            Keyword = keyword,
            SelectedId = selected?.MaDanhMuc,
            SelectedCategory = selected,
            FormCategory = selected is not null ? new DanhMuc { MaDanhMuc = selected.MaDanhMuc, TenDanhMuc = selected.TenDanhMuc, MoTa = selected.MoTa } : new DanhMuc()
        };

        return View("~/Views/Admin/Categories.cshtml", model);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCategory(DanhMuc category)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        try
        {
            category.TenDanhMuc = category.TenDanhMuc?.Trim() ?? string.Empty;
            category.MoTa = category.MoTa?.Trim();

            if (string.IsNullOrWhiteSpace(category.TenDanhMuc))
            {
                TempData["Error"] = "Ten danh muc khong duoc de trong.";
                return RedirectToAction(nameof(Categories), new { selectedId = category.MaDanhMuc });
            }

            var existing = !string.IsNullOrWhiteSpace(category.MaDanhMuc) ? await Context.DanhMucs.FirstOrDefaultAsync(x => x.MaDanhMuc == category.MaDanhMuc) : null;
            if (existing is null)
            {
                category.MaDanhMuc = string.IsNullOrWhiteSpace(category.MaDanhMuc) ? $"DM{DateTime.Now:MMddHHmmss}" : category.MaDanhMuc.Trim().ToUpperInvariant();
                category.DaXoa = false;
                Context.DanhMucs.Add(category);
            }
            else
            {
                existing.TenDanhMuc = category.TenDanhMuc;
                existing.MoTa = category.MoTa;
            }

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories), new { selectedId = category.MaDanhMuc });
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the luu danh muc. Vui long kiem tra du lieu nhap.";
            return RedirectToAction(nameof(Categories), new { selectedId = category.MaDanhMuc });
        }
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var category = await Context.DanhMucs.Include(x => x.SanPhams).FirstOrDefaultAsync(x => x.MaDanhMuc == id);
        if (category is null) return RedirectToAction(nameof(Categories));
        if (category.SanPhams.Any())
        {
            TempData["Error"] = "Khong the an danh muc khi van con san pham dang su dung.";
            return RedirectToAction(nameof(Categories), new { selectedId = id });
        }

        category.DaXoa = true;
        await Context.SaveChangesAsync();
        TempData["Success"] = "Da an danh muc khoi danh sach hien thi.";
        return RedirectToAction(nameof(Categories));
    }
}
