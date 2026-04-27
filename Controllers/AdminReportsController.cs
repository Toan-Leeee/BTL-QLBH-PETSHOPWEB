using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;

namespace SieuPetMvc.Controllers;

[Route("admin/reports")]
public class AdminReportsController : AdminBaseController
{
    public AdminReportsController(ApplicationDbContext context) : base(context)
    {
    }

    [HttpGet("")]
    public async Task<IActionResult> Reports()
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminPage("Admin.Reports");

        ViewBag.TotalRevenue = await Context.DonHangs.Where(x => x.TrangThai == "Hoan tat").SumAsync(x => (decimal?)x.TongTien) ?? 0;
        ViewBag.TotalOrders = await Context.DonHangs.CountAsync();
        ViewBag.TotalProducts = await Context.SanPhams.CountAsync();
        ViewBag.ActiveProducts = await Context.SanPhams.CountAsync(x => x.TrangThai);
        ViewBag.TotalEmployees = await Context.NhanViens.CountAsync();
        ViewBag.TotalCategories = await Context.DanhMucs.CountAsync();
        ViewBag.TotalCustomers = await Context.KhachHangs.CountAsync();
        ViewBag.TotalImports = await Context.PhieuNhaps.CountAsync();
        return View("~/Views/Admin/Reports.cshtml");
    }
}
