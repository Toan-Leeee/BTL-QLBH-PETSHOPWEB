using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class AdminImportsViewModel
{
    public IEnumerable<PhieuNhap> Receipts { get; set; } = Enumerable.Empty<PhieuNhap>();
    public IEnumerable<SanPham> Products { get; set; } = Enumerable.Empty<SanPham>();
    public PhieuNhap FormReceipt { get; set; } = new()
    {
        NgayNhap = DateTime.Now
    };

    public PhieuNhap? SelectedReceipt { get; set; }
    public string? Keyword { get; set; }
    public string? SelectedId { get; set; }
}
