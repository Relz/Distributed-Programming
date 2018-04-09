using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace TextListener
{
	class Program
	{
		private const string exchangeName = "backend-api";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(exchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				string text = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Text}{textId}");
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Text}{textId}: {text}' from redis database({Redis.Instance.Database.Database})");
				Console.WriteLine($"{textId}: {text}");
			});

			Console.WriteLine("TextListener has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
