using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace TextRankCalc
{
	class Program
	{
		private const string _listeningExchangeName = "processing-limiter";
		private const string _publishExchangeName = "text-rank-tasks";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_listeningExchangeName);
			rabbitMq.ConsumeQueue(textIdAndStatus =>
			{
				Console.WriteLine($"New message from {_listeningExchangeName}: \"{textIdAndStatus}\"");

				string[] splittedMessage = textIdAndStatus.Split(ConstantLibrary.RabbitMq.Delimiter);
				string textId = splittedMessage[0];
				bool doesTextAllowed = splittedMessage[1] == ConstantLibrary.RabbitMq.ProcessingLimiter.Status.True;

				if (!doesTextAllowed)
				{
					Console.WriteLine($"{textId} not allowed");
					return;
				}
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Status}{textId}: {ConstantLibrary.Redis.Status.Processing}' to redis database({Redis.Instance.Database.Database})");
				Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Status}{textId}", ConstantLibrary.Redis.Status.Processing);

				// System.Threading.Thread.Sleep(5000);

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
