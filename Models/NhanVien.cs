using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblNhanVien")]
public class NhanVien
{
    [Key, StringLength(15)]
    [Column("PK_MaNV")]
    public string MaNhanVien { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Column("sHoTen")]
    public string HoTen { get; set; } = string.Empty;

    [StringLength(15)]
    [Column("sSoDienThoai")]
    public string? SoDienThoai { get; set; }

    [Required, StringLength(100)]
    [Column("sEmail")]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(255)]
    [Column("sMatKhau")]
    public string MatKhau { get; set; } = string.Empty;

    [Required, StringLength(50)]
    [Column("sVaiTro")]
    public string VaiTro { get; set; } = string.Empty;

    [Column("dNgayVaoLam")]
    public DateTime? NgayVaoLam { get; set; }

    [Column("bDaXoa")]
    public bool DaXoa { get; set; }

    public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    public ICollection<PhieuNhap> PhieuNhaps { get; set; } = new List<PhieuNhap>();
}
