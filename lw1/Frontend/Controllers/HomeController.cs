using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;

namespace Frontend.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index(FormModel formModel)
		{
			// TODO: Send formModel.Data to Backend, recieve id and display it for user
			return View(formModel);
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
