using Microsoft.AspNetCore.Mvc;
using SieuPetMvc.Data;

namespace SieuPetMvc.Controllers;

public abstract class AdminBaseController : Controller
{
    protected readonly ApplicationDbContext Context;

    protected AdminBaseController(ApplicationDbContext context)
    {
        Context = context;
    }

    protected bool EnsureAdmin()
    {
        return !string.IsNullOrWhiteSpace(HttpContext.Session.GetString("EmployeeId"));
    }

    protected void UseAdminPage(string assetName)
    {
        ViewBag.Layout = "_AdminLayout";
        ViewBag.PageAssetName = assetName;
    }
}
