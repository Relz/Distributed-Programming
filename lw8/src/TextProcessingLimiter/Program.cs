using System;
using RabbitMqLibrary;
using RedisLibrary;
using System.Collections.Generic;

namespace TextProcessingLimiter
{
	class Program
	{
		private const string _textListeningExchangeName = "backend-api";
		private const string _textMarkersListeningExchangeName = "text-markers";
		private const string _publishExchangeName = "text-processing";

		private const int _limit = 3;

		private static IDictionary<string, int> _textSuccessMarkersCount = new Dictionary<string, int>();

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_textListeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_textListeningExchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Console.WriteLine($"New message from {_textListeningExchangeName}: \"{textId}\"");
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Status}{textId}: processing' to redis database({Redis.Instance.Database.Database})");
				Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Status}{textId}", "processing");

				int textSuccessMarkerCount;
				if (!_textSuccessMarkersCount.TryGetValue(textId, out textSuccessMarkerCount) || textSuccessMarkerCount != _limit)
				{
					Console.WriteLine($"{textId} to {_publishExchangeName} exchange");
					rabbitMq.PublishToExchange(_publishExchangeName, textId);
				}

				Console.WriteLine("----------");
			});

			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_textMarkersListeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_textMarkersListeningExchangeName);
			rabbitMq.ConsumeQueue(textIdMarker =>
			{
				Console.WriteLine($"New message from {_textMarkersListeningExchangeName}: \"{textIdMarker}\"");
				string[] data = textIdMarker.Split('|');
				bool isTextSucceeded = data[1] == "true";
				if (isTextSucceeded)
				{
					var textId = data[0];
					if (!_textSuccessMarkersCount.ContainsKey(textId))
					{
						_textSuccessMarkersCount.Add(textId, 0);
					}
					++_textSuccessMarkersCount[textId];
					int textSuccessMarkerCount;
					_textSuccessMarkersCount.TryGetValue(textId, out textSuccessMarkerCount);
					Console.WriteLine($"{textId}: {textSuccessMarkerCount}");
				}

				Console.WriteLine("----------");
			});

			Console.WriteLine("TextProcessingLimiter has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
