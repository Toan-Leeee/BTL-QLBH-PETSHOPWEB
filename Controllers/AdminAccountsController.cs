using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

[Route("admin/accounts")]
public class AdminAccountsController : AdminBaseController
{
    public AdminAccountsController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Accounts(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Accounts");

        var query = Context.NhanViens.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.HoTen.Contains(keyword) || x.Email.Contains(keyword) || (x.SoDienThoai ?? string.Empty).Contains(keyword) || x.VaiTro.Contains(keyword));
        }

        var employees = await query.OrderBy(x => x.HoTen).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? employees.FirstOrDefault(x => x.MaNhanVien == selectedId) : employees.FirstOrDefault();

        var model = new AdminAccountsViewModel
        {
            Employees = employees,
            Keyword = keyword,
            SelectedId = selected?.MaNhanVien,
            SelectedEmployee = selected,
            FormEmployee = selected is not null
                ? new NhanVien { MaNhanVien = selected.MaNhanVien, HoTen = selected.HoTen, Email = selected.Email, SoDienThoai = selected.SoDienThoai, VaiTro = selected.VaiTro, MatKhau = selected.MatKhau, NgayVaoLam = selected.NgayVaoLam }
                : new NhanVien { VaiTro = "Nhan vien", NgayVaoLam = DateTime.Today }
        };

        return View("~/Views/Admin/Accounts.cshtml", model);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEmployee(NhanVien employee)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        try
        {
            employee.HoTen = employee.HoTen?.Trim() ?? string.Empty;
            employee.Email = employee.Email?.Trim() ?? string.Empty;
            employee.SoDienThoai = employee.SoDienThoai?.Trim();
            employee.VaiTro = employee.VaiTro?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(employee.HoTen) || string.IsNullOrWhiteSpace(employee.Email))
            {
                TempData["Error"] = "Ho ten va email nhan vien khong duoc de trong.";
                return RedirectToAction(nameof(Accounts), new { selectedId = employee.MaNhanVien });
            }

            var existing = !string.IsNullOrWhiteSpace(employee.MaNhanVien) ? await Context.NhanViens.FirstOrDefaultAsync(x => x.MaNhanVien == employee.MaNhanVien) : null;
            if (existing is null)
            {
                employee.MaNhanVien = string.IsNullOrWhiteSpace(employee.MaNhanVien) ? $"NV{DateTime.Now:MMddHHmmss}" : employee.MaNhanVien.Trim().ToUpperInvariant();
                employee.MatKhau = string.IsNullOrWhiteSpace(employee.MatKhau) ? "1234567" : employee.MatKhau;
                employee.NgayVaoLam ??= DateTime.Today;
                employee.DaXoa = false;
                Context.NhanViens.Add(employee);
            }
            else
            {
                existing.HoTen = employee.HoTen;
                existing.Email = employee.Email;
                existing.SoDienThoai = employee.SoDienThoai;
                existing.VaiTro = employee.VaiTro;
                existing.NgayVaoLam = employee.NgayVaoLam;
                if (!string.IsNullOrWhiteSpace(employee.MatKhau)) existing.MatKhau = employee.MatKhau;
            }

            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Accounts), new { selectedId = employee.MaNhanVien });
        }
        catch (Exception)
        {
            TempData["Error"] = "Khong the luu tai khoan nhan vien. Vui long kiem tra du lieu nhap.";
            return RedirectToAction(nameof(Accounts), new { selectedId = employee.MaNhanVien });
        }
    }

    [HttpPost("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEmployee(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        var employee = await Context.NhanViens.FirstOrDefaultAsync(x => x.MaNhanVien == id);
        if (employee is null) return RedirectToAction(nameof(Accounts));

        var relatedOrders = await Context.DonHangs.Where(x => x.MaNhanVien == id).ToListAsync();
        foreach (var order in relatedOrders)
        {
            order.MaNhanVien = null;
        }

        employee.DaXoa = true;
        await Context.SaveChangesAsync();
        TempData["Success"] = "Da an tai khoan nhan vien khoi danh sach.";
        return RedirectToAction(nameof(Accounts));
    }
}
