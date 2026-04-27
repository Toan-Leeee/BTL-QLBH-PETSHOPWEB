using Microsoft.AspNetCore.Mvc;

namespace SieuPetMvc.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction("Products", "AdminProducts");
    }
}
