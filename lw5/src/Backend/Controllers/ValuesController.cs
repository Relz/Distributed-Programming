using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RabbitMqLibrary;
using RedisLibrary;

namespace Backend.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		private RabbitMq _rabbitMq = new RabbitMq();

		// GET api/values/<id>
		[HttpGet("{id}")]
		public string Get(string id)
		{
			return Redis.Instance.Database.StringGet(id);
		}

		// POST api/values
		[HttpPost]
		public string Post([FromBody]DataDto value)
		{
			string id = Guid.NewGuid().ToString();
			Redis.Instance.Database.StringSet(id, value.Data);
			const string exchangeName = "backend-api";
			_rabbitMq.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
			_rabbitMq.PublishToExchange(exchangeName, id);
			return id;
		}
	}
}
