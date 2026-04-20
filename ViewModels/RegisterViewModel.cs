using System.ComponentModel.DataAnnotations;

namespace SieuPetMvc.ViewModels;

public class RegisterViewModel
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận chưa khớp")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Bạn cần đồng ý điều khoản")]
    public bool AgreeTerms { get; set; }
}
