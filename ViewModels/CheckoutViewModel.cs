using System.ComponentModel.DataAnnotations;

namespace SieuPetMvc.ViewModels;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui long nhap ho va ten")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap so dien thoai")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap dia chi")]
    public string Address { get; set; } = string.Empty;

    public CartViewModel Cart { get; set; } = new();
}
