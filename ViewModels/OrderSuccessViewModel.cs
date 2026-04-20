using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class OrderSuccessViewModel
{
    public DonHang Order { get; set; } = new();
    public List<CartItemViewModel> Items { get; set; } = new();
}
