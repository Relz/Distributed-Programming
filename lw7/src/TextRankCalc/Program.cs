using System;
using RabbitMqLibrary;

namespace TextRankCalc
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
			rabbitMq.ConsumeQueue(id =>
			{
				Console.WriteLine($"{id} to text-rank-tasks queue");
				rabbitMq.PublishToExchange("text-rank-tasks", id);
			});

			Console.WriteLine("TextRankCalc has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
