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
			rabbitMq.ConsumeQueue(message =>
			{
				Console.WriteLine($"{message}: {Redis.Instance.Database.StringGet(message)}");
			});

			Console.WriteLine("TextListener has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
