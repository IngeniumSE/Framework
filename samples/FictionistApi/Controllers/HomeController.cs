using Ingenium.Platform.Data;
using Ingenium.Platform.Security.Users;

using Microsoft.AspNetCore.Mvc;

namespace FictionistApi.Controllers;
public class HomeController : Controller
{
	readonly IUserReader _users;

	public HomeController(IUserReader users)
	{
		_users = users;
	}

	public async Task<IActionResult> Index()
	{
		var system = await _users.FindByKeyAsync(new UserId(0));

		return Json(system);
	}

	[HttpPost]
	public IActionResult Index([FromBody] User user)
	{
		return Json(user);
	}
}
