using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(ProductFilterViewModel filter)
    {
        ViewBag.BodyClass = "catalog-page";

        var query = _context.SanPhams
            .Include(x => x.DanhMuc)
            .Where(x => x.TrangThai)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(x => x.TenSanPham.Contains(filter.Keyword) || (x.MoTa ?? string.Empty).Contains(filter.Keyword));
        }

        if (!string.IsNullOrWhiteSpace(filter.CategoryId))
        {
            query = query.Where(x => x.MaDanhMuc == filter.CategoryId);
        }

        filter.Page = Math.Max(filter.Page, 1);
        var totalItems = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)filter.PageSize));
        if (filter.Page > totalPages)
        {
            filter.Page = totalPages;
        }

        var products = await query
            .OrderBy(x => x.MaDanhMuc)
            .ThenBy(x => x.GiaBan)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var categories = await _context.DanhMucs.OrderBy(x => x.TenDanhMuc).ToListAsync();
        var pageTitle = "Tất cả sản phẩm";
        if (!string.IsNullOrWhiteSpace(filter.CategoryId))
        {
            pageTitle = categories.FirstOrDefault(x => x.MaDanhMuc == filter.CategoryId)?.TenDanhMuc ?? pageTitle;
        }
        else if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            pageTitle = $"Kết quả cho \"{filter.Keyword}\"";
        }

        var vm = new ProductListViewModel
        {
            Products = products,
            Categories = categories,
            Filter = filter,
            CurrentPage = filter.Page,
            TotalPages = totalPages,
            PageTitle = pageTitle
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(string id)
    {
        ViewBag.BodyClass = "details-page";

        var product = await _context.SanPhams
            .Include(x => x.DanhMuc)
            .FirstOrDefaultAsync(x => x.MaSanPham == id && x.TrangThai);
        if (product is null)
        {
            return NotFound();
        }

        var vm = new ProductDetailsViewModel
        {
            Product = product,
            RelatedProducts = await _context.SanPhams
                .Where(x => x.TrangThai && x.MaDanhMuc == product.MaDanhMuc && x.MaSanPham != id)
                .Take(4)
                .ToListAsync(),
            CustomerStories = new[]
            {
                new CustomerStoryViewModel { Title = "Khách hàng 1", ImageUrl = "/images/stories/story-1.svg" },
                new CustomerStoryViewModel { Title = "Khách hàng 2", ImageUrl = "/images/stories/story-2.svg" },
                new CustomerStoryViewModel { Title = "Khách hàng 3", ImageUrl = "/images/stories/story-3.svg" },
                new CustomerStoryViewModel { Title = "Khách hàng 4", ImageUrl = "/images/stories/story-4.svg" },
                new CustomerStoryViewModel { Title = "Khách hàng 5", ImageUrl = "/images/stories/story-5.svg" }
            }
        };

        return View(vm);
    }
}
