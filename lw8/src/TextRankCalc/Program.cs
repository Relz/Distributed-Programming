using System;
using RabbitMqLibrary;

namespace TextRankCalc
{
	class Program
	{
		private const string _listeningExchangeName = "backend-api";
		private const string _publishExchangeName = "text-rank-tasks";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_listeningExchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Console.WriteLine($"New message from {_listeningExchangeName}: \"{textId}\"");
				Console.WriteLine($"{textId} to {_publishExchangeName} exchange");
				rabbitMq.PublishToExchange(_publishExchangeName, textId);

				Console.WriteLine("----------");
			});

			Console.WriteLine("TextRankCalc has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
