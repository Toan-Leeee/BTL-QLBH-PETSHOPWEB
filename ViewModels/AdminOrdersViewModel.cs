using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class AdminOrdersViewModel
{
    public IEnumerable<DonHang> Orders { get; set; } = Enumerable.Empty<DonHang>();
    public DonHang? SelectedOrder { get; set; }
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public string? SelectedId { get; set; }

    public IEnumerable<string> StatusOptions { get; set; } = new[]
    {
        "Cho xac nhan",
        "Cho thanh toan",
        "Dang giao",
        "Hoan tat",
        "Da huy"
    };
}
