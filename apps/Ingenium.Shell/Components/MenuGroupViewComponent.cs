using Ingenium.Platform.Shell;

using Microsoft.AspNetCore.Mvc;

namespace Ingenium.Shell.Components;

public class MenuGroupViewComponent : ViewComponent
{
	readonly IMenuProvider[] _providers;

  public MenuGroupViewComponent(IMenuProvider[] providers)
  {
		_providers = Ensure.IsNotNull(providers, nameof(providers));
  }

	public async Task<IViewComponentResult> InvokeAsync(string groupId, string viewName)
	{
		Ensure.IsNotNullOrEmpty(viewName, nameof(viewName));

		List<Menu> menus = new();
		foreach (var provider in _providers)
		{
			menus.AddRange(await provider.GetMenusAsync(new MenuGroupId(groupId)));
		}

		// TODO: Merge and group menus.

		return View(viewName, menus);
	}
}