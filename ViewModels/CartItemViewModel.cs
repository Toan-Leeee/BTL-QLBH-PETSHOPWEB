namespace SieuPetMvc.ViewModels;

public class CartItemViewModel
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Label { get; set; } = "Thú cưng";
    public int Quantity { get; set; }
    public int AvailableStock { get; set; }
    public decimal Price { get; set; }
    public decimal LineTotal => Price * Quantity;
}
