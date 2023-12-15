using Microsoft.AspNetCore.Mvc;

namespace ms18_applicatie.Controllers;

public class DeclaratiesController : Controller
{
    public DeclaratiesController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Edit(long id)
    {
        ViewData["id"] = id;
        return View();
    }

    public IActionResult Nieuw()
    {
        return View();
    }
}