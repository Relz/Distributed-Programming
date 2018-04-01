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
			Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(id));
			string result = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Text}{id}");
			Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Text}{id}: {result}' from redis database({Redis.Instance.Database.Database})");
			return result;
		}

		// POST api/values
		[HttpPost]
		public string Post([FromBody]DataDto value)
		{
			string textId = Guid.NewGuid().ToString();
			Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
			Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Text}{textId}", value.Data);
			Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Text}{textId}: {value.Data}' to redis database({Redis.Instance.Database.Database})");
			const string exchangeName = "backend-api";
			_rabbitMq.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
			_rabbitMq.PublishToExchange(exchangeName, textId);
			return textId;
		}
	}
}
