using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Frontend.Models;
using ModelLibrary;

namespace Frontend.Controllers
{
	public class HomeController : Controller
	{
		public async Task<IActionResult> Index(FormModel formModel)
		{
			if (formModel.Data != null)
			{
				string id;
				string url = "http://127.0.0.1:5050/api/values";
				StringContent stringContent = new StringContent($"{{ \"data\": \"{formModel.Data}\"}}", Encoding.UTF8, "application/json");
				using (HttpClient httpClient = new HttpClient())
				{
					httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
					using (HttpResponseMessage response = await httpClient.PostAsync(url, stringContent))
					using (HttpContent content = response.Content)
					{
						id = content.ReadAsStringAsync().Result;
					}
				}
				TextDetails textDetails = new TextDetails();
				textDetails.Id = id;

				return RedirectToAction("TextDetails", "Home", textDetails);
			}
			return View(formModel);
		}

		public async Task<IActionResult> TextDetails(TextDetails textDetails)
		{
			TextDetails result = null;
			if (textDetails.Id != null)
			{
				uint retries = 0;
				while (result == null && retries != 5)
				{
					result = await TryToGetTextDetails(textDetails.Id);
					if (result == null)
					{
						++retries;
						System.Threading.Thread.Sleep(2000);
					}
				}
			}
			return View(result);
		}

		private async Task<TextDetails> TryToGetTextDetails(string id)
		{
			string url = $"http://127.0.0.1:5050/api/text_details/{id}";
			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
				using (HttpResponseMessage response = await httpClient.GetAsync(url))
				using (HttpContent content = response.Content)
				{
					return JsonConvert.DeserializeObject<TextDetails>(content.ReadAsStringAsync().Result);
				}
			}
			return null;
		}

		public async Task<IActionResult> Statistics(StatisticsModel statisticsModel)
		{
			string url = $"http://127.0.0.1:5050/api/statistics";
			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
				using (HttpResponseMessage response = await httpClient.GetAsync(url))
				using (HttpContent content = response.Content)
				{
					Statistics statistics = JsonConvert.DeserializeObject<Statistics>(content.ReadAsStringAsync().Result);
					statisticsModel.TotalTextCount = statistics.TotalTextCount;
					statisticsModel.HighRankCount = statistics.HighRankCount;
					statisticsModel.AverageRank = statistics.TotalTextCount == 0 ? 0 : statistics.TotalRank / statistics.TotalTextCount;
				}
			}
			return View(statisticsModel);
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
