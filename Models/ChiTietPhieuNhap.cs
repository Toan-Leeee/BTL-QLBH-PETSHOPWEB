using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblChiTietPhieuNhap")]
public class ChiTietPhieuNhap
{
    [Column("FK_MaPN")]
    public string MaPhieuNhap { get; set; } = string.Empty;

    [Column("FK_MaSP")]
    public string MaSanPham { get; set; } = string.Empty;

    [Column("iSoLuongNhap")]
    public int SoLuongNhap { get; set; }

    [Column("mGiaNhap")]
    public decimal GiaNhap { get; set; }

    public PhieuNhap? PhieuNhap { get; set; }
    public SanPham? SanPham { get; set; }
}
