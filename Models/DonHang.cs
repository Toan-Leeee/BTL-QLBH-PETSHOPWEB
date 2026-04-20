using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblDonHang")]
public class DonHang
{
    [Key, StringLength(15)]
    [Column("PK_MaDH")]
    public string MaDonHang { get; set; } = string.Empty;

    [StringLength(15)]
    [Column("FK_MaNV")]
    public string? MaNhanVien { get; set; }

    [StringLength(15)]
    [Column("FK_MaKH")]
    public string? MaKhachHang { get; set; }

    [Column("dNgayTao")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [Required, StringLength(50)]
    [Column("sTrangThai")]
    public string TrangThai { get; set; } = "Cho xac nhan";

    [Column("mTongTien")]
    public decimal TongTien { get; set; }

    public NhanVien? NhanVien { get; set; }
    public KhachHang? KhachHang { get; set; }
    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
}
