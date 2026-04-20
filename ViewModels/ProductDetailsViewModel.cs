using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class ProductDetailsViewModel
{
    public SanPham Product { get; set; } = new();
    public IEnumerable<SanPham> RelatedProducts { get; set; } = Enumerable.Empty<SanPham>();
    public IEnumerable<CustomerStoryViewModel> CustomerStories { get; set; } = Enumerable.Empty<CustomerStoryViewModel>();
}
