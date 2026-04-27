using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Login(LoginViewModel model)
    {
        ViewBag.BodyClass = "auth-page";

        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var employee = _context.NhanViens.FirstOrDefault(x =>
            (x.Email == model.EmailOrPhone || x.SoDienThoai == model.EmailOrPhone)
            && x.MatKhau == model.Password);

        if (employee != null)
        {
            HttpContext.Session.SetString("UserName", employee.HoTen);
            HttpContext.Session.SetString("UserRole", employee.VaiTro);
            HttpContext.Session.SetString("EmployeeId", employee.MaNhanVien);

            return RedirectToAction("Products", "AdminProducts");
        }

        var customer = _context.KhachHangs.FirstOrDefault(x =>
            (x.Email == model.EmailOrPhone || x.SoDienThoai == model.EmailOrPhone)
            && x.MatKhau == model.Password);

        if (customer != null)
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
    public IActionResult Register(RegisterViewModel model)
    {
        ViewBag.BodyClass = "auth-page";

        if (ModelState.IsValid == false)
        {
            return View(model);
        }

        var checkCustomer = _context.KhachHangs.FirstOrDefault(x =>
            x.Email == model.Email || x.SoDienThoai == model.Phone);

        if (checkCustomer != null)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc số điện thoại đã tồn tại.");
            return View(model);
        }

        var customer = new KhachHang
        {
            MaKhachHang = $"KH{DateTime.Now:yyMMddHHmmssf}",
            TenKhachHang = model.FullName,
            Email = model.Email,
            SoDienThoai = model.Phone,
            MatKhau = model.Password
        };

        _context.KhachHangs.Add(customer);
        _context.SaveChanges();

        TempData["Success"] = "Đăng ký tài khoản thành công. Hãy đăng nhập để tiếp tục.";
        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
