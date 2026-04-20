using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Services;

public interface ICartService
{
    Task<CartViewModel> GetCartAsync();
    Task AddAsync(string productId, int quantity = 1);
    Task UpdateAsync(string productId, int quantity);
    Task RemoveAsync(string productId);
    Task ClearAsync();
}
