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
			if (!Redis.Instance.Database.KeyExists(id))
			{
				return "Invalid id";
			}
			string text = Redis.Instance.Database.StringGet(id);
			if (!Redis.Instance.Database.KeyExists(text))
			{
				return "Hasn't calculated yet";
			}
			return Redis.Instance.Database.StringGet(text);
		}
	}
}
