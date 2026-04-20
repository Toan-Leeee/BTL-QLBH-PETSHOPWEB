using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblKhachHang")]
public class KhachHang
{
    [Key, StringLength(15)]
    [Column("PK_MaKH")]
    public string MaKhachHang { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Column("sTenKH")]
    public string TenKhachHang { get; set; } = string.Empty;

    [StringLength(15)]
    [Column("sSoDienThoai")]
    public string? SoDienThoai { get; set; }

    [StringLength(100)]
    [Column("sEmail")]
    public string? Email { get; set; }

    [Required, StringLength(255)]
    [Column("sMatKhau")]
    public string MatKhau { get; set; } = string.Empty;

    public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
