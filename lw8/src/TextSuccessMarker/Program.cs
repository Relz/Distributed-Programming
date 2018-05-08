using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace TextListener
{
	class Program
	{
		private const string _listeningExchangeName = "text-rank-calc";
		private const string _publishExchangeName = "text-success-marker";

		private const double _successLowerBound = 0.5;

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_listeningExchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Console.WriteLine($"New message from {_listeningExchangeName}: \"{textId}\"");

				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				double rank = Double.Parse(Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Rank}{textId}"));
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{textId}: {rank}' from redis database({Redis.Instance.Database.Database})");

				var stringToPublish = $"{textId}{ConstantLibrary.RabbitMq.Delimiter}" +
					(rank > _successLowerBound
						? ConstantLibrary.RabbitMq.TextSuccessMarker.Status.True
						: ConstantLibrary.RabbitMq.TextSuccessMarker.Status.False);

				Console.WriteLine($"{stringToPublish} to {_publishExchangeName} exchange");
				rabbitMq.PublishToExchange(_publishExchangeName, stringToPublish);

				Console.WriteLine("----------");
			});

			Console.WriteLine($"TextSuccessMarker has started. Success lower bound is {_successLowerBound}");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
