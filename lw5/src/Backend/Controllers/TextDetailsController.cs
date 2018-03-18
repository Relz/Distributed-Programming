using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	[Route("api/text_details")]
	public class TextDetailsController : Controller
	{
		// GET api/text_details/<id>
		[HttpGet("{id}")]
		public string Get(string id)
		{
			if (!RedisHelper.Instance.Exists(id))
			{
				return "Invalid id";
			}
			string text = RedisHelper.Instance.Get(id);
			if (!RedisHelper.Instance.Exists(text))
			{
				return "Hasn't calculated yet";
			}
			return RedisHelper.Instance.Get(text);
		}
	}
}
