using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Models;

namespace SieuPetMvc.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch
        {
        }

        if (await context.DanhMucs.AnyAsync())
        {
            return;
        }

        context.DanhMucs.AddRange(
            new DanhMuc { MaDanhMuc = "DM001", TenDanhMuc = "Cho nho", MoTa = "Cac giong cho nho, de cham soc" },
            new DanhMuc { MaDanhMuc = "DM002", TenDanhMuc = "Cho con", MoTa = "Cac be cun trong giai doan so sinh den 3 thang" },
            new DanhMuc { MaDanhMuc = "DM003", TenDanhMuc = "Phu kien", MoTa = "Do choi, bat an, day dat" }
        );

        context.SanPhams.AddRange(
            new SanPham { MaSanPham = "SP001", MaDanhMuc = "DM001", TenSanPham = "M0231 - Pomeranian Trang", GiaBan = 6900000, SoLuongTonKho = 1, MoTa = "Giong duc, 02 thang, mau trang", HinhAnh = "/images/products/pomeranian-white.svg", TrangThai = true },
            new SanPham { MaSanPham = "SP002", MaDanhMuc = "DM001", TenSanPham = "M0502 - Poodle Tiny Vang", GiaBan = 3900000, SoLuongTonKho = 1, MoTa = "Giong cai, 02 thang, mau vang kem", HinhAnh = "/images/products/poodle-gold.svg", TrangThai = true },
            new SanPham { MaSanPham = "SP003", MaDanhMuc = "DM001", TenSanPham = "M0102 - Poodle Tiny Sepia", GiaBan = 4000000, SoLuongTonKho = 1, MoTa = "Giong duc, 02 thang, mau nau do", HinhAnh = "/images/products/poodle-brown.svg", TrangThai = true },
            new SanPham { MaSanPham = "SP004", MaDanhMuc = "DM002", TenSanPham = "M0512 - Alaskan Malamute", GiaBan = 8900000, SoLuongTonKho = 1, MoTa = "Giong duc, 03 thang, mau xam trang", HinhAnh = "/images/products/alaskan.svg", TrangThai = true },
            new SanPham { MaSanPham = "SP005", MaDanhMuc = "DM002", TenSanPham = "M0128 - Pembroke Corgi", GiaBan = 7900000, SoLuongTonKho = 1, MoTa = "Giong duc, 02 thang, chan ngan", HinhAnh = "/images/products/corgi.svg", TrangThai = true },
            new SanPham { MaSanPham = "SP006", MaDanhMuc = "DM002", TenSanPham = "#1000078 - Shiba Inu Nau Do", GiaBan = 12000000, SoLuongTonKho = 1, MoTa = "Giong cai, 2 thang, nau do", HinhAnh = "/images/products/shiba.svg", TrangThai = true },
            new SanPham { MaSanPham = "PK001", MaDanhMuc = "DM003", TenSanPham = "Bat an chong truot", GiaBan = 320000, SoLuongTonKho = 18, MoTa = "Bat an cho thu cung", HinhAnh = "/images/products/bowl.svg", TrangThai = true },
            new SanPham { MaSanPham = "PK002", MaDanhMuc = "DM003", TenSanPham = "Day dat di dao", GiaBan = 180000, SoLuongTonKho = 25, MoTa = "Size M, mau navy", HinhAnh = "/images/products/leash.svg", TrangThai = true }
        );

        context.NhanViens.AddRange(
            new NhanVien { MaNhanVien = "NV001", HoTen = "Le Xuan Toan", SoDienThoai = "0912345678", Email = "admin@sieupet.vn", MatKhau = "123456", VaiTro = "Admin", NgayVaoLam = DateTime.Today.AddYears(-1) },
            new NhanVien { MaNhanVien = "NV002", HoTen = "Nguyen Minh Anh", SoDienThoai = "0909111222", Email = "ops@sieupet.vn", MatKhau = "123456", VaiTro = "Van hanh", NgayVaoLam = DateTime.Today.AddMonths(-10) },
            new NhanVien { MaNhanVien = "NV003", HoTen = "Le Thu Ha", SoDienThoai = "0909888777", Email = "support@sieupet.vn", MatKhau = "123456", VaiTro = "CSKH", NgayVaoLam = DateTime.Today.AddMonths(-6) }
        );

        context.KhachHangs.AddRange(
            new KhachHang { MaKhachHang = "KH001", TenKhachHang = "Le Thu Ha", SoDienThoai = "0909888777", Email = "khachhang@sieupet.vn", MatKhau = "123456" },
            new KhachHang { MaKhachHang = "KH002", TenKhachHang = "Nguyen Minh Anh", SoDienThoai = "0911222333", Email = "minhanh@gmail.com", MatKhau = "123456" },
            new KhachHang { MaKhachHang = "KH003", TenKhachHang = "Tran Quoc Bao", SoDienThoai = "0988333444", Email = "quocbao@gmail.com", MatKhau = "123456" }
        );

        context.DonHangs.AddRange(
            new DonHang { MaDonHang = "DH260411001", MaKhachHang = "KH002", MaNhanVien = "NV001", NgayTao = DateTime.Now.AddHours(-4), TrangThai = "Hoan tat", TongTien = 12200000 },
            new DonHang { MaDonHang = "DH260411002", MaKhachHang = "KH001", MaNhanVien = "NV002", NgayTao = DateTime.Now.AddHours(-2), TrangThai = "Cho xac nhan", TongTien = 8900000 },
            new DonHang { MaDonHang = "DH260411003", MaKhachHang = "KH003", MaNhanVien = "NV003", NgayTao = DateTime.Now.AddHours(-1), TrangThai = "Cho thanh toan", TongTien = 500000 }
        );

        context.ChiTietDonHangs.AddRange(
            new ChiTietDonHang { MaChiTietDonHang = "CT001", MaDonHang = "DH260411001", MaSanPham = "SP006", SoLuong = 1, DonGiaBan = 12000000, ThanhTien = 12000000 },
            new ChiTietDonHang { MaChiTietDonHang = "CT002", MaDonHang = "DH260411001", MaSanPham = "PK001", SoLuong = 1, DonGiaBan = 320000, ThanhTien = 320000 },
            new ChiTietDonHang { MaChiTietDonHang = "CT003", MaDonHang = "DH260411001", MaSanPham = "PK002", SoLuong = 1, DonGiaBan = 180000, ThanhTien = 180000 },
            new ChiTietDonHang { MaChiTietDonHang = "CT004", MaDonHang = "DH260411002", MaSanPham = "SP004", SoLuong = 1, DonGiaBan = 8900000, ThanhTien = 8900000 },
            new ChiTietDonHang { MaChiTietDonHang = "CT005", MaDonHang = "DH260411003", MaSanPham = "PK001", SoLuong = 1, DonGiaBan = 320000, ThanhTien = 320000 },
            new ChiTietDonHang { MaChiTietDonHang = "CT006", MaDonHang = "DH260411003", MaSanPham = "PK002", SoLuong = 1, DonGiaBan = 180000, ThanhTien = 180000 }
        );

        context.PhieuNhaps.AddRange(
            new PhieuNhap { MaPhieuNhap = "PN001", MaNhanVien = "NV001", NgayNhap = DateTime.Now.AddDays(-10), NhaCungCap = "Trai giong Happy Paws" },
            new PhieuNhap { MaPhieuNhap = "PN002", MaNhanVien = "NV002", NgayNhap = DateTime.Now.AddDays(-6), NhaCungCap = "Kho phu kien Pet World" },
            new PhieuNhap { MaPhieuNhap = "PN003", MaNhanVien = "NV002", NgayNhap = DateTime.Now.AddDays(-2), NhaCungCap = "Nha phan phoi Doggy House" }
        );

        context.ChiTietPhieuNhaps.AddRange(
            new ChiTietPhieuNhap { MaPhieuNhap = "PN001", MaSanPham = "SP004", SoLuongNhap = 1, GiaNhap = 7200000 },
            new ChiTietPhieuNhap { MaPhieuNhap = "PN001", MaSanPham = "SP005", SoLuongNhap = 1, GiaNhap = 6300000 },
            new ChiTietPhieuNhap { MaPhieuNhap = "PN002", MaSanPham = "PK001", SoLuongNhap = 20, GiaNhap = 190000 },
            new ChiTietPhieuNhap { MaPhieuNhap = "PN002", MaSanPham = "PK002", SoLuongNhap = 30, GiaNhap = 95000 },
            new ChiTietPhieuNhap { MaPhieuNhap = "PN003", MaSanPham = "SP006", SoLuongNhap = 1, GiaNhap = 9800000 }
        );

        await context.SaveChangesAsync();
    }
}
