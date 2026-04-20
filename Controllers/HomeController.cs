using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SieuPetMvc.Data;
using SieuPetMvc.ViewModels;

namespace SieuPetMvc.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.BodyClass = "home-page";

        var model = new HomeViewModel
        {
            FeaturedProducts = await _context.SanPhams
                .Where(x => x.TrangThai)
                .OrderBy(x => x.GiaBan)
                .Take(8)
                .ToListAsync(),
            Articles = new List<KnowledgeArticleViewModel>
            {
                new() { Title = "Pomeranian là gì? Cách nhận biết chó Pomeranian", Summary = "Tổng quan giống chó Pom, tính cách, cách chăm và chi phí nuôi.", ImageUrl = "/images/articles/pom.svg" },
                new() { Title = "Chế độ ăn cho chó bạn cần biết", Summary = "Một vài nguyên tắc dinh dưỡng nền tảng cho chó con và chó trưởng thành.", ImageUrl = "/images/articles/food.svg" },
                new() { Title = "Tại sao chó cần phá đồ đạc và cách phòng ngừa", Summary = "Hành vi cắn phá đồ vật và cách huấn luyện để hạn chế trong nhà.", ImageUrl = "/images/articles/training.svg" }
            }
        };

        return View(model);
    }
}
