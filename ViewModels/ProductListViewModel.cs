using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class ProductListViewModel
{
    public IEnumerable<SanPham> Products { get; set; } = Enumerable.Empty<SanPham>();
    public IEnumerable<DanhMuc> Categories { get; set; } = Enumerable.Empty<DanhMuc>();
    public ProductFilterViewModel Filter { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string PageTitle { get; set; } = "Sản phẩm";
}
