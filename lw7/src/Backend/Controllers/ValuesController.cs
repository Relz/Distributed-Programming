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
		private const string _publishExchangeName = "backend-api";

		private RabbitMq _rabbitMq = new RabbitMq();

		public ValuesController()
		{
			_rabbitMq.ExchangeDeclare(_publishExchangeName, ExchangeType.Fanout);
		}

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

			Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Text}{textId}: {value.Data}' to redis database({Redis.Instance.Database.Database})");
			Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Text}{textId}", value.Data);

			Console.WriteLine($"{textId} to {_publishExchangeName} exchange");
			_rabbitMq.PublishToExchange(_publishExchangeName, textId);

			return textId;
		}
	}
}
