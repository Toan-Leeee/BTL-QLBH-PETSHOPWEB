using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class AdminCustomersViewModel
{
    public IEnumerable<KhachHang> Customers { get; set; } = Enumerable.Empty<KhachHang>();
    public KhachHang FormCustomer { get; set; } = new();
    public KhachHang? SelectedCustomer { get; set; }
    public string? Keyword { get; set; }
    public string? SelectedId { get; set; }
}
