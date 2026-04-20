using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SieuPetMvc.Models;

[Table("tblDanhMuc")]
public class DanhMuc
{
    [Key, StringLength(15)]
    [Column("PK_MaDM")]
    public string MaDanhMuc { get; set; } = string.Empty;

    [Required, StringLength(100)]
    [Column("sTenDM")]
    public string TenDanhMuc { get; set; } = string.Empty;

    [StringLength(255)]
    [Column("sMoTa")]
    public string? MoTa { get; set; }

    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
