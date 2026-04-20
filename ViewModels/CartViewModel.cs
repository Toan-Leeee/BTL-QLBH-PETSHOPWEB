namespace SieuPetMvc.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal GrandTotal => SubTotal + ShippingFee;
}
