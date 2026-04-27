using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblPhieuNhap")]
public class PhieuNhap
{
    [Key, StringLength(15)]
    [Column("PK_MaPN")]
    public string MaPhieuNhap { get; set; } = string.Empty;

    [Required, StringLength(15)]
    [Column("FK_MaNV")]
    public string MaNhanVien { get; set; } = string.Empty;

    [Column("dNgayNhap")]
    public DateTime NgayNhap { get; set; } = DateTime.Now;

    [StringLength(200)]
    [Column("sNhaCungCap")]
    public string? NhaCungCap { get; set; }

    [Column("bDaXoa")]
    public bool DaXoa { get; set; }

    public NhanVien? NhanVien { get; set; }
    public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
}
