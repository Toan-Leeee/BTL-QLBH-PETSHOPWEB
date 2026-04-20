using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblChiTietDonHang")]
public class ChiTietDonHang
{
    [Key, StringLength(15)]
    [Column("PK_MaCTDH")]
    public string MaChiTietDonHang { get; set; } = string.Empty;

    [Required, StringLength(15)]
    [Column("FK_MaDH")]
    public string MaDonHang { get; set; } = string.Empty;

    [Required, StringLength(15)]
    [Column("FK_MaSP")]
    public string MaSanPham { get; set; } = string.Empty;

    [Column("iSoLuong")]
    public int SoLuong { get; set; }

    [Column("mDonGiaBan")]
    public decimal DonGiaBan { get; set; }

    [Column("mThanhTien")]
    public decimal ThanhTien { get; set; }

    public DonHang? DonHang { get; set; }
    public SanPham? SanPham { get; set; }
}
