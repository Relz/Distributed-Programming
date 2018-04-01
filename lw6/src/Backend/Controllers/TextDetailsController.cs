using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisLibrary;

namespace Backend.Controllers
{
	[Route("api/text_details")]
	public class TextDetailsController : Controller
	{
		// GET api/text_details/<id>
		[HttpGet("{id}")]
		public string Get(string id)
		{
			Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(id));
			if (!Redis.Instance.Database.KeyExists($"{ConstantLibrary.Redis.Prefix.Text}{id}"))
			{
				return "Invalid id";
			}
			if (!Redis.Instance.Database.KeyExists($"{ConstantLibrary.Redis.Prefix.Rank}{id}"))
			{
				return "Hasn't calculated yet";
			}
			string result = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Rank}{id}");
			Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{id}: {result}' from redis database({Redis.Instance.Database.Database})");
			return result;
		}
	}
}
