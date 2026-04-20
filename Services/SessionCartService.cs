using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.Extensions;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Services;

public class SessionCartService : ICartService
{
    private const string CartKey = "SieuPet.Cart";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public SessionCartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task AddAsync(string productId, int quantity = 1)
    {
        if (quantity <= 0)
        {
            return;
        }

        var product = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaSanPham == productId && x.TrangThai);
        if (product is null)
        {
            return;
        }

        var cart = GetRawCart();
        var current = cart.TryGetValue(productId, out var oldValue) ? oldValue : 0;
        var nextQuantity = current + quantity;
        if (product.SoLuongTonKho > 0)
        {
            nextQuantity = Math.Min(nextQuantity, product.SoLuongTonKho);
        }

        cart[productId] = nextQuantity;
        SaveRawCart(cart);
    }

    public async Task UpdateAsync(string productId, int quantity)
    {
        var cart = GetRawCart();
        if (quantity <= 0)
        {
            cart.Remove(productId);
            SaveRawCart(cart);
            return;
        }

        var product = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaSanPham == productId);
        if (product is null)
        {
            cart.Remove(productId);
        }
        else
        {
            cart[productId] = product.SoLuongTonKho > 0 ? Math.Min(quantity, product.SoLuongTonKho) : quantity;
        }

        SaveRawCart(cart);
    }

    public async Task RemoveAsync(string productId)
    {
        var cart = GetRawCart();
        cart.Remove(productId);
        SaveRawCart(cart);
        await Task.CompletedTask;
    }

    public async Task ClearAsync()
    {
        _httpContextAccessor.HttpContext!.Session.Remove(CartKey);
        await Task.CompletedTask;
    }

    public async Task<CartViewModel> GetCartAsync()
    {
        var raw = GetRawCart();
        var ids = raw.Keys.ToList();
        var products = await _context.SanPhams
            .Where(x => ids.Contains(x.MaSanPham))
            .ToDictionaryAsync(x => x.MaSanPham, x => x);

        var model = new CartViewModel();
        foreach (var id in ids)
        {
            if (!products.TryGetValue(id, out var product))
            {
                continue;
            }

            var quantity = raw[id];
            model.Items.Add(new CartItemViewModel
            {
                ProductId = product.MaSanPham,
                Name = product.TenSanPham,
                ImageUrl = product.HinhAnh ?? "/images/products/default.svg",
                Subtitle = product.MoTa ?? string.Empty,
                Label = product.MaDanhMuc == "DM003" ? "Phu kien" : "Thu cung",
                Quantity = quantity,
                Price = product.GiaBan
            });
        }

        model.SubTotal = model.Items.Sum(x => x.LineTotal);
        model.ShippingFee = 0;
        return model;
    }

    private Dictionary<string, int> GetRawCart()
    {
        return _httpContextAccessor.HttpContext!.Session.GetObject<Dictionary<string, int>>(CartKey) ?? new Dictionary<string, int>();
    }

    private void SaveRawCart(Dictionary<string, int> cart)
    {
        _httpContextAccessor.HttpContext!.Session.SetObject(CartKey, cart);
    }
}
