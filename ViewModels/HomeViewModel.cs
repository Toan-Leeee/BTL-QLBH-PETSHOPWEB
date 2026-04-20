using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class HomeViewModel
{
    public List<SanPham> FeaturedProducts { get; set; } = new();
    public List<KnowledgeArticleViewModel> Articles { get; set; } = new();
}
