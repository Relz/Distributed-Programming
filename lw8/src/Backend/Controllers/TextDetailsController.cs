using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RedisLibrary;
using ModelLibrary;

namespace Backend.Controllers
{
	[Route("api/text_details")]
	public class TextDetailsController : Controller
	{
		// GET api/text_details/<id>
		[HttpGet("{id}")]
		public TextDetails Get(string id)
		{
			Console.WriteLine($"Get text details for id: {id}");
			TextDetails result = new TextDetails();
			result.Id = id;
			Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(id));
			if (!Redis.Instance.Database.KeyExists($"{ConstantLibrary.Redis.Prefix.Text}{id}"))
			{
				result.Status = "Invalid text id";
			}
			result.Status = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Status}{id}");
			result.Rank = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Rank}{id}");
			Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{id}: {result.Rank}' from redis database({Redis.Instance.Database.Database})");

			return result;
		}
	}
}
