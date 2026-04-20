using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Models;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool EnsureAdmin() => !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("EmployeeId"));

    private void UseAdminLayout() => ViewBag.Layout = "_AdminLayout";

    public async Task<IActionResult> Products(string? keyword, string? statusFilter, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        var categories = await _context.DanhMucs.OrderBy(x => x.TenDanhMuc).ToListAsync();
        var query = _context.SanPhams.Include(x => x.DanhMuc).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaSanPham.Contains(keyword) || x.TenSanPham.Contains(keyword) || (x.MoTa ?? string.Empty).Contains(keyword));
        }

        if (statusFilter == "active") query = query.Where(x => x.TrangThai);
        if (statusFilter == "hidden") query = query.Where(x => !x.TrangThai);

        var products = await query.OrderByDescending(x => x.TrangThai).ThenBy(x => x.TenSanPham).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? products.FirstOrDefault(x => x.MaSanPham == selectedId) : null;

        return View(new AdminProductsViewModel
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
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProduct(SanPham product)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        product.HinhAnh = string.IsNullOrWhiteSpace(product.HinhAnh) ? "/images/products/default.svg" : product.HinhAnh.Trim();
        if (string.IsNullOrWhiteSpace(product.MaDanhMuc) || string.IsNullOrWhiteSpace(product.TenSanPham))
        {
            TempData["Error"] = "Vui long nhap day du ten san pham va danh muc.";
            return RedirectToAction(nameof(Products), new { selectedId = product.MaSanPham });
        }

        var existing = !string.IsNullOrWhiteSpace(product.MaSanPham) ? await _context.SanPhams.FindAsync(product.MaSanPham) : null;
        if (existing is null)
        {
            var prefix = product.MaDanhMuc == "DM003" ? "PK" : "SP";
            product.MaSanPham = string.IsNullOrWhiteSpace(product.MaSanPham) ? $"{prefix}{DateTime.Now:yyMMddHHmmss}" : product.MaSanPham.Trim().ToUpperInvariant();
            product.TenSanPham = product.TenSanPham.Trim();
            _context.SanPhams.Add(product);
            TempData["Success"] = "Da them san pham moi.";
        }
        else
        {
            existing.MaDanhMuc = product.MaDanhMuc;
            existing.TenSanPham = product.TenSanPham.Trim();
            existing.GiaBan = product.GiaBan;
            existing.SoLuongTonKho = product.SoLuongTonKho;
            existing.MoTa = product.MoTa?.Trim();
            existing.HinhAnh = product.HinhAnh;
            existing.TrangThai = product.TrangThai;
            TempData["Success"] = "Da cap nhat san pham.";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Products), new { selectedId = product.MaSanPham });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var product = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == id);
        if (product is null) return RedirectToAction(nameof(Products));

        var orderDetails = await _context.ChiTietDonHangs.Where(x => x.MaSanPham == id).ToListAsync();
        var importDetails = await _context.ChiTietPhieuNhaps.Where(x => x.MaSanPham == id).ToListAsync();

        _context.ChiTietDonHangs.RemoveRange(orderDetails);
        _context.ChiTietPhieuNhaps.RemoveRange(importDetails);
        _context.SanPhams.Remove(product);

        await _context.SaveChangesAsync();
        TempData["Success"] = "Da xoa san pham khoi database.";
        return RedirectToAction(nameof(Products));
    }

    public async Task<IActionResult> Orders(string? keyword, string? status, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        var query = _context.DonHangs
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

        return View(new AdminOrdersViewModel
        {
            Orders = orders,
            Keyword = keyword,
            Status = status,
            SelectedId = selectedOrder?.MaDonHang,
            SelectedOrder = selectedOrder
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrder(string id, string status)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var order = await _context.DonHangs.FirstOrDefaultAsync(x => x.MaDonHang == id);
        if (order is null) return RedirectToAction(nameof(Orders));
        order.TrangThai = status;
        order.MaNhanVien ??= HttpContext.Session.GetString("EmployeeId");
        await _context.SaveChangesAsync();
        TempData["Success"] = "Da cap nhat don hang.";
        return RedirectToAction(nameof(Orders), new { selectedId = id });
    }

    public async Task<IActionResult> Accounts(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        var query = _context.NhanViens.AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.HoTen.Contains(keyword) || x.Email.Contains(keyword) || (x.SoDienThoai ?? string.Empty).Contains(keyword) || x.VaiTro.Contains(keyword));
        }

        var employees = await query.OrderBy(x => x.HoTen).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? employees.FirstOrDefault(x => x.MaNhanVien == selectedId) : employees.FirstOrDefault();

        return View(new AdminAccountsViewModel
        {
            Employees = employees,
            Keyword = keyword,
            SelectedId = selected?.MaNhanVien,
            SelectedEmployee = selected,
            FormEmployee = selected is not null
                ? new NhanVien { MaNhanVien = selected.MaNhanVien, HoTen = selected.HoTen, Email = selected.Email, SoDienThoai = selected.SoDienThoai, VaiTro = selected.VaiTro, MatKhau = selected.MatKhau, NgayVaoLam = selected.NgayVaoLam }
                : new NhanVien { VaiTro = "Nhan vien", NgayVaoLam = DateTime.Today }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEmployee(NhanVien employee)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var existing = !string.IsNullOrWhiteSpace(employee.MaNhanVien) ? await _context.NhanViens.FindAsync(employee.MaNhanVien) : null;
        if (existing is null)
        {
            employee.MaNhanVien = string.IsNullOrWhiteSpace(employee.MaNhanVien) ? $"NV{DateTime.Now:yyMMddHHmmss}" : employee.MaNhanVien.Trim().ToUpperInvariant();
            employee.MatKhau = string.IsNullOrWhiteSpace(employee.MatKhau) ? "123456" : employee.MatKhau;
            employee.NgayVaoLam ??= DateTime.Today;
            _context.NhanViens.Add(employee);
        }
        else
        {
            existing.HoTen = employee.HoTen.Trim();
            existing.Email = employee.Email.Trim();
            existing.SoDienThoai = employee.SoDienThoai?.Trim();
            existing.VaiTro = employee.VaiTro.Trim();
            existing.NgayVaoLam = employee.NgayVaoLam;
            if (!string.IsNullOrWhiteSpace(employee.MatKhau)) existing.MatKhau = employee.MatKhau;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Accounts), new { selectedId = employee.MaNhanVien });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEmployee(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        var employee = await _context.NhanViens.FirstOrDefaultAsync(x => x.MaNhanVien == id);
        if (employee is null) return RedirectToAction(nameof(Accounts));

        var relatedOrders = await _context.DonHangs.Where(x => x.MaNhanVien == id).ToListAsync();
        foreach (var order in relatedOrders)
        {
            order.MaNhanVien = null;
        }

        var receipts = await _context.PhieuNhaps.Where(x => x.MaNhanVien == id).ToListAsync();
        var receiptIds = receipts.Select(x => x.MaPhieuNhap).ToList();
        var receiptDetails = await _context.ChiTietPhieuNhaps.Where(x => receiptIds.Contains(x.MaPhieuNhap)).ToListAsync();

        _context.ChiTietPhieuNhaps.RemoveRange(receiptDetails);
        _context.PhieuNhaps.RemoveRange(receipts);
        _context.NhanViens.Remove(employee);

        await _context.SaveChangesAsync();
        TempData["Success"] = "Da xoa tai khoan khoi database.";
        return RedirectToAction(nameof(Accounts));
    }

    public async Task<IActionResult> Categories(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        var query = _context.DanhMucs.Include(x => x.SanPhams).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaDanhMuc.Contains(keyword) || x.TenDanhMuc.Contains(keyword) || (x.MoTa ?? string.Empty).Contains(keyword));
        }

        var categories = await query.OrderBy(x => x.TenDanhMuc).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? categories.FirstOrDefault(x => x.MaDanhMuc == selectedId) : categories.FirstOrDefault();

        return View(new AdminCategoriesViewModel
        {
            Categories = categories,
            Keyword = keyword,
            SelectedId = selected?.MaDanhMuc,
            SelectedCategory = selected,
            FormCategory = selected is not null ? new DanhMuc { MaDanhMuc = selected.MaDanhMuc, TenDanhMuc = selected.TenDanhMuc, MoTa = selected.MoTa } : new DanhMuc()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCategory(DanhMuc category)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var existing = !string.IsNullOrWhiteSpace(category.MaDanhMuc) ? await _context.DanhMucs.FindAsync(category.MaDanhMuc) : null;
        if (existing is null)
        {
            category.MaDanhMuc = string.IsNullOrWhiteSpace(category.MaDanhMuc) ? $"DM{DateTime.Now:yyMMddHHmmss}" : category.MaDanhMuc.Trim().ToUpperInvariant();
            _context.DanhMucs.Add(category);
        }
        else
        {
            existing.TenDanhMuc = category.TenDanhMuc.Trim();
            existing.MoTa = category.MoTa?.Trim();
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Categories), new { selectedId = category.MaDanhMuc });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var category = await _context.DanhMucs.Include(x => x.SanPhams).FirstOrDefaultAsync(x => x.MaDanhMuc == id);
        if (category is null || category.SanPhams.Any()) return RedirectToAction(nameof(Categories), new { selectedId = id });
        _context.DanhMucs.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Categories));
    }

    public async Task<IActionResult> Customers(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        var query = _context.KhachHangs.Include(x => x.DonHangs).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaKhachHang.Contains(keyword) || x.TenKhachHang.Contains(keyword) || (x.Email ?? string.Empty).Contains(keyword) || (x.SoDienThoai ?? string.Empty).Contains(keyword));
        }

        var customers = await query.OrderBy(x => x.TenKhachHang).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? customers.FirstOrDefault(x => x.MaKhachHang == selectedId) : customers.FirstOrDefault();

        return View(new AdminCustomersViewModel
        {
            Customers = customers,
            Keyword = keyword,
            SelectedId = selected?.MaKhachHang,
            SelectedCustomer = selected,
            FormCustomer = selected is not null ? new KhachHang { MaKhachHang = selected.MaKhachHang, TenKhachHang = selected.TenKhachHang, SoDienThoai = selected.SoDienThoai, Email = selected.Email, MatKhau = selected.MatKhau } : new KhachHang()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCustomer(KhachHang customer)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        var existing = !string.IsNullOrWhiteSpace(customer.MaKhachHang) ? await _context.KhachHangs.FindAsync(customer.MaKhachHang) : null;
        if (existing is null)
        {
            customer.MaKhachHang = string.IsNullOrWhiteSpace(customer.MaKhachHang) ? $"KH{DateTime.Now:yyMMddHHmmss}" : customer.MaKhachHang.Trim().ToUpperInvariant();
            customer.MatKhau = string.IsNullOrWhiteSpace(customer.MatKhau) ? "123456" : customer.MatKhau;
            _context.KhachHangs.Add(customer);
        }
        else
        {
            existing.TenKhachHang = customer.TenKhachHang.Trim();
            existing.Email = customer.Email?.Trim();
            existing.SoDienThoai = customer.SoDienThoai?.Trim();
            if (!string.IsNullOrWhiteSpace(customer.MatKhau)) existing.MatKhau = customer.MatKhau;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Customers), new { selectedId = customer.MaKhachHang });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCustomer(string id)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        var customer = await _context.KhachHangs.FirstOrDefaultAsync(x => x.MaKhachHang == id);
        if (customer is null) return RedirectToAction(nameof(Customers));

        var orders = await _context.DonHangs.Where(x => x.MaKhachHang == id).ToListAsync();
        var orderIds = orders.Select(x => x.MaDonHang).ToList();
        var orderDetails = await _context.ChiTietDonHangs.Where(x => orderIds.Contains(x.MaDonHang)).ToListAsync();

        _context.ChiTietDonHangs.RemoveRange(orderDetails);
        _context.DonHangs.RemoveRange(orders);
        _context.KhachHangs.Remove(customer);

        await _context.SaveChangesAsync();
        TempData["Success"] = "Da xoa khach hang khoi database.";
        return RedirectToAction(nameof(Customers));
    }

    public async Task<IActionResult> Imports(string? keyword, string? selectedId)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        var query = _context.PhieuNhaps.Include(x => x.NhanVien).Include(x => x.ChiTietPhieuNhaps).ThenInclude(x => x.SanPham).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.MaPhieuNhap.Contains(keyword) || (x.NhaCungCap ?? string.Empty).Contains(keyword) || (x.NhanVien != null && x.NhanVien.HoTen.Contains(keyword)));
        }

        var receipts = await query.OrderByDescending(x => x.NgayNhap).ToListAsync();
        var selected = !string.IsNullOrWhiteSpace(selectedId) ? receipts.FirstOrDefault(x => x.MaPhieuNhap == selectedId) : receipts.FirstOrDefault();

        return View(new AdminImportsViewModel
        {
            Receipts = receipts,
            Products = await _context.SanPhams.OrderBy(x => x.TenSanPham).ToListAsync(),
            Keyword = keyword,
            SelectedId = selected?.MaPhieuNhap,
            SelectedReceipt = selected,
            FormReceipt = new PhieuNhap { NgayNhap = DateTime.Now }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveImport(PhieuNhap receipt, List<string>? productIds, List<int>? quantities, List<decimal>? importPrices)
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");

        productIds ??= new List<string>();
        quantities ??= new List<int>();
        importPrices ??= new List<decimal>();

        var validLines = productIds.Select((productId, index) => new
        {
            ProductId = productId,
            Quantity = index < quantities.Count ? quantities[index] : 0,
            Price = index < importPrices.Count ? importPrices[index] : 0m
        }).Where(x => !string.IsNullOrWhiteSpace(x.ProductId) && x.Quantity > 0 && x.Price >= 0).ToList();

        if (!validLines.Any()) return RedirectToAction(nameof(Imports));

        receipt.MaPhieuNhap = string.IsNullOrWhiteSpace(receipt.MaPhieuNhap) ? $"PN{DateTime.Now:yyMMddHHmmss}" : receipt.MaPhieuNhap.Trim().ToUpperInvariant();
        receipt.NgayNhap = receipt.NgayNhap == default ? DateTime.Now : receipt.NgayNhap;
        receipt.MaNhanVien = HttpContext.Session.GetString("EmployeeId") ?? "NV001";
        _context.PhieuNhaps.Add(receipt);

        foreach (var line in validLines)
        {
            var product = await _context.SanPhams.FirstOrDefaultAsync(x => x.MaSanPham == line.ProductId);
            if (product is null) continue;
            product.SoLuongTonKho += line.Quantity;
            if (product.SoLuongTonKho > 0) product.TrangThai = true;

            _context.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
            {
                MaPhieuNhap = receipt.MaPhieuNhap,
                MaSanPham = line.ProductId,
                SoLuongNhap = line.Quantity,
                GiaNhap = line.Price
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Imports), new { selectedId = receipt.MaPhieuNhap });
    }

    public async Task<IActionResult> Reports()
    {
        if (!EnsureAdmin()) return RedirectToAction("Login", "Account");
        UseAdminLayout();

        ViewBag.TotalRevenue = await _context.DonHangs.Where(x => x.TrangThai == "Hoan tat").SumAsync(x => (decimal?)x.TongTien) ?? 0;
        ViewBag.TotalOrders = await _context.DonHangs.CountAsync();
        ViewBag.TotalProducts = await _context.SanPhams.CountAsync();
        ViewBag.ActiveProducts = await _context.SanPhams.CountAsync(x => x.TrangThai);
        ViewBag.TotalEmployees = await _context.NhanViens.CountAsync();
        ViewBag.TotalCategories = await _context.DanhMucs.CountAsync();
        ViewBag.TotalCustomers = await _context.KhachHangs.CountAsync();
        ViewBag.TotalImports = await _context.PhieuNhaps.CountAsync();
        return View();
    }
}
