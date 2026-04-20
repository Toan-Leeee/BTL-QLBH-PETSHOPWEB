using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        ViewBag.BodyClass = "auth-page";
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewBag.BodyClass = "auth-page";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var employee = await _context.NhanViens.FirstOrDefaultAsync(x =>
            (x.Email == model.EmailOrPhone || x.SoDienThoai == model.EmailOrPhone) && x.MatKhau == model.Password);
        if (employee is not null)
        {
            HttpContext.Session.SetString("UserName", employee.HoTen);
            HttpContext.Session.SetString("UserRole", employee.VaiTro);
            HttpContext.Session.SetString("EmployeeId", employee.MaNhanVien);
            return RedirectToAction("Products", "Admin");
        }

        var customer = await _context.KhachHangs.FirstOrDefaultAsync(x =>
            (x.Email == model.EmailOrPhone || x.SoDienThoai == model.EmailOrPhone) && x.MatKhau == model.Password);
        if (customer is not null)
        {
            HttpContext.Session.SetString("UserName", customer.TenKhachHang);
            HttpContext.Session.SetString("UserRole", "Customer");
            HttpContext.Session.SetString("CustomerId", customer.MaKhachHang);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Sai thông tin đăng nhập.");
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.BodyClass = "auth-page";
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewBag.BodyClass = "auth-page";

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _context.KhachHangs.AnyAsync(x => x.Email == model.Email || x.SoDienThoai == model.Phone))
        {
            ModelState.AddModelError(string.Empty, "Email hoặc số điện thoại đã tồn tại.");
            return View(model);
        }

        var customer = new KhachHang
        {
            MaKhachHang = $"KH{DateTime.Now:yyyyMMddHHmmss}",
            TenKhachHang = model.FullName,
            Email = model.Email,
            SoDienThoai = model.Phone,
            MatKhau = model.Password
        };

        _context.KhachHangs.Add(customer);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Tạo tài khoản thành công. Hãy đăng nhập để tiếp tục.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
