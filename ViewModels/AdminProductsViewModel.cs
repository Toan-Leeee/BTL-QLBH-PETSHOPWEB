using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class AdminProductsViewModel
{
    public IEnumerable<SanPham> Products { get; set; } = Enumerable.Empty<SanPham>();
    public IEnumerable<DanhMuc> Categories { get; set; } = Enumerable.Empty<DanhMuc>();
    public SanPham FormProduct { get; set; } = new()
    {
        TrangThai = true,
        SoLuongTonKho = 1,
        HinhAnh = "/images/products/image 22.jpg"
    };

    public string? Keyword { get; set; }
    public string? StatusFilter { get; set; }
    public string? SelectedId { get; set; }
}
