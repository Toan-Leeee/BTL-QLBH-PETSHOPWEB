using SieuPetMvc.Models;

namespace SieuPetMvc.ViewModels;

public class AdminAccountsViewModel
{
    public IEnumerable<NhanVien> Employees { get; set; } = Enumerable.Empty<NhanVien>();
    public NhanVien FormEmployee { get; set; } = new()
    {
        VaiTro = "Nhân viên",
        NgayVaoLam = DateTime.Today
    };

    public NhanVien? SelectedEmployee { get; set; }
    public string? Keyword { get; set; }
    public string? SelectedId { get; set; }
}
