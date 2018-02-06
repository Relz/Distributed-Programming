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
		private static readonly ConcurrentDictionary<string, string> _data = new ConcurrentDictionary<string, string>();

		// GET api/values/<id>
		[HttpGet("{id}")]
		public string Get(string id)
		{
			string data = null;
			_data.TryGetValue(id, out data);
			return data;
		}

		// POST api/values
		[HttpPost]
		public string Post([FromBody]DataDto value)
		{
			string id = Guid.NewGuid().ToString();
			_data[id] = value.Data;
			return id;
		}
	}
}
