using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblSanPham")]
public class SanPham
{
    [Key, StringLength(15)]
    [Column("PK_MaSP")]
    public string MaSanPham { get; set; } = string.Empty;

    [Required, StringLength(15)]
    [Column("FK_MaDM")]
    public string MaDanhMuc { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Column("sTenSP")]
    public string TenSanPham { get; set; } = string.Empty;

    [Column("mGiaBan")]
    public decimal GiaBan { get; set; }

    [Column("iSoLuongTonKho")]
    public int SoLuongTonKho { get; set; }

    [Column("sMoTa")]
    public string? MoTa { get; set; }

    [StringLength(255)]
    [Column("sHinhAnh")]
    public string? HinhAnh { get; set; }

    [Column("bTrangThai")]
    public bool TrangThai { get; set; } = true;

    public DanhMuc? DanhMuc { get; set; }
    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
}
