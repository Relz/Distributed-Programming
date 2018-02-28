using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace Frontend.Controllers
{
	public class HomeController : Controller
	{
		public async Task<IActionResult> Index(FormModel formModel)
		{
			if (formModel.Data != null)
			{
				string url = "http://127.0.0.1:5050/api/values";
				StringContent stringContent = new StringContent($"{{ \"data\": \"{formModel.Data}\"}}", Encoding.UTF8, "application/json");
				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					using (HttpResponseMessage response = await httpClient.PostAsync(url, stringContent))
					using (HttpContent content = response.Content)
					{
						formModel.Id = content.ReadAsStringAsync().Result;
					}
				}
			}
			return View(formModel);
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
