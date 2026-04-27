using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Models;

namespace SieuPetMvc.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<NhanVien> NhanViens => Set<NhanVien>();
    public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
    public DbSet<DanhMuc> DanhMucs => Set<DanhMuc>();
    public DbSet<SanPham> SanPhams => Set<SanPham>();
    public DbSet<DonHang> DonHangs => Set<DonHang>();
    public DbSet<ChiTietDonHang> ChiTietDonHangs => Set<ChiTietDonHang>();
    public DbSet<PhieuNhap> PhieuNhaps => Set<PhieuNhap>();
    public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps => Set<ChiTietPhieuNhap>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChiTietPhieuNhap>()
            .HasKey(x => new { x.MaPhieuNhap, x.MaSanPham });

        modelBuilder.Entity<SanPham>()
            .Property(x => x.GiaBan)
            .HasPrecision(18, 2);

        modelBuilder.Entity<DonHang>()
            .Property(x => x.TongTien)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ChiTietDonHang>()
            .Property(x => x.DonGiaBan)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ChiTietDonHang>()
            .Property(x => x.ThanhTien)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ChiTietPhieuNhap>()
            .Property(x => x.GiaNhap)
            .HasPrecision(18, 2);

        modelBuilder.Entity<KhachHang>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<KhachHang>()
            .HasIndex(x => x.SoDienThoai)
            .IsUnique();

        modelBuilder.Entity<NhanVien>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<SanPham>()
            .HasQueryFilter(x => !x.DaXoa);

        modelBuilder.Entity<DanhMuc>()
            .HasQueryFilter(x => !x.DaXoa);

        modelBuilder.Entity<KhachHang>()
            .HasQueryFilter(x => !x.DaXoa);

        modelBuilder.Entity<NhanVien>()
            .HasQueryFilter(x => !x.DaXoa);

        modelBuilder.Entity<DonHang>()
            .HasQueryFilter(x => !x.DaXoa);

        modelBuilder.Entity<PhieuNhap>()
            .HasQueryFilter(x => !x.DaXoa);

        modelBuilder.Entity<SanPham>()
            .HasOne(x => x.DanhMuc)
            .WithMany(x => x.SanPhams)
            .HasForeignKey(x => x.MaDanhMuc);

        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(x => x.DonHang)
            .WithMany(x => x.ChiTietDonHangs)
            .HasForeignKey(x => x.MaDonHang);

        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(x => x.SanPham)
            .WithMany(x => x.ChiTietDonHangs)
            .HasForeignKey(x => x.MaSanPham);

        modelBuilder.Entity<PhieuNhap>()
            .HasOne(x => x.NhanVien)
            .WithMany(x => x.PhieuNhaps)
            .HasForeignKey(x => x.MaNhanVien);

        modelBuilder.Entity<ChiTietPhieuNhap>()
            .HasOne(x => x.PhieuNhap)
            .WithMany(x => x.ChiTietPhieuNhaps)
            .HasForeignKey(x => x.MaPhieuNhap);

        modelBuilder.Entity<ChiTietPhieuNhap>()
            .HasOne(x => x.SanPham)
            .WithMany(x => x.ChiTietPhieuNhaps)
            .HasForeignKey(x => x.MaSanPham);

        modelBuilder.Entity<DonHang>()
            .HasOne(x => x.KhachHang)
            .WithMany(x => x.DonHangs)
            .HasForeignKey(x => x.MaKhachHang)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<DonHang>()
            .HasOne(x => x.NhanVien)
            .WithMany(x => x.DonHangs)
            .HasForeignKey(x => x.MaNhanVien)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
