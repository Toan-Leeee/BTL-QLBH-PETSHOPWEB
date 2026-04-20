using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class AdminCategoriesViewModel
{
    public IEnumerable<DanhMuc> Categories { get; set; } = Enumerable.Empty<DanhMuc>();
    public DanhMuc FormCategory { get; set; } = new();
    public DanhMuc? SelectedCategory { get; set; }
    public string? Keyword { get; set; }
    public string? SelectedId { get; set; }
}
