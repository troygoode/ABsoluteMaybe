using System.Web.Mvc;

namespace ABsoluteMaybe.SampleWebsite.MVC2.Controllers
{
	[HandleError]
	public class HomeController : Controller
	{
		public ViewResult Index()
		{
			var msg =
				"Welcome to the ABsoluteMaybe Sample Website! Some people see this in proper case, some see it in all caps.";
			if (ABsoluteMaybe.Test("Homepage_Message_Casing",
			                       "Purchased Bumper Sticker",
			                       new[] {"ProperCase", "ALL_CAPS"}) == "ALL_CAPS")
				msg = msg.ToUpperInvariant();

			ViewData["Message"] = msg;

			return View();
		}

		public RedirectToRouteResult BeggingTotallyWorks()
		{
			ABsoluteMaybe.Convert("Begging_Totally_Works");
			return RedirectToAction("ShoppingCart");
		}

		public ViewResult ShoppingCart()
		{
			var shippingCost = ABsoluteMaybe.Test("Free_Shipping", "Purchased Bumper Sticker", new[] {0m, 9.99m});
			return View(new ShoppingCartViewModel
			            	{
								BumperStickerCost = 59.99m,
			            		ShippingCost = shippingCost
			            	});
		}

		[HttpPut, ValidateAntiForgeryToken]
		public RedirectToRouteResult ShoppingCart(string form)
		{
			ABsoluteMaybe.Convert("Purchased Bumper Sticker");
			return RedirectToAction("ThankYou");
		}

		public ViewResult ThankYou()
		{
			return View();
		}

		#region Nested type: ShoppingCartViewModel

		public class ShoppingCartViewModel
		{
			public decimal BumperStickerCost { get; set; }
			public decimal ShippingCost { get; set; }
		}

		#endregion
	}
}