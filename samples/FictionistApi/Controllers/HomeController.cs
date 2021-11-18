using Microsoft.AspNetCore.Mvc;

namespace FictionistApi.Controllers;
public class HomeController : Controller
{
	public IActionResult Index()
	{
		return View();
	}
}
