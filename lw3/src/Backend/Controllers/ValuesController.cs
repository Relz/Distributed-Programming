using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		// GET api/values/<id>
		[HttpGet("{id}")]
		public string Get(string id)
		{
			return RedisHelper.Instance.Get(id);
		}

		// POST api/values
		[HttpPost]
		public string Post([FromBody]DataDto value)
		{
			string id = Guid.NewGuid().ToString();
			RedisHelper.Instance.Set(id, value.Data);
			return id;
		}
	}
}
