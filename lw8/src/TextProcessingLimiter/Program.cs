using System;
using RabbitMqLibrary;
using RedisLibrary;
using System.Collections.Generic;

namespace TextProcessingLimiter
{
	class Program
	{
		private const string _textListeningExchangeName = "backend-api";
		private const string _textMarkersListeningExchangeName = "text-success-marker";
		private const string _publishExchangeName = "processing-limiter";

		private const int _limit = 3;

		private static int _succeededTextCount = 0;

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

				bool status = _succeededTextCount < _limit;
				if (status)
				{
					++_succeededTextCount;
					Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Status}{textId}: {ConstantLibrary.Redis.Status.Accepted}' to redis database({Redis.Instance.Database.Database})");
					Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Status}{textId}", ConstantLibrary.Redis.Status.Accepted);
				}
				else
				{
					Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Status}{textId}: {ConstantLibrary.Redis.Status.Rejected}' to redis database({Redis.Instance.Database.Database})");
					Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Status}{textId}", ConstantLibrary.Redis.Status.Rejected);
				}
				var stringToPublish = $"{textId}{ConstantLibrary.RabbitMq.Delimiter}" +
					(status ? ConstantLibrary.RabbitMq.ProcessingLimiter.Status.True : ConstantLibrary.RabbitMq.ProcessingLimiter.Status.False);
				Console.WriteLine($"{stringToPublish} to {_publishExchangeName} exchange");
				rabbitMq.PublishToExchange(_publishExchangeName, stringToPublish);

				Console.WriteLine("----------");
			});

			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare(_textMarkersListeningExchangeName, ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange(_textMarkersListeningExchangeName);
			rabbitMq.ConsumeQueue(textIdMarker =>
			{
				Console.WriteLine($"New message from {_textMarkersListeningExchangeName}: \"{textIdMarker}\"");

				string[] data = textIdMarker.Split(ConstantLibrary.RabbitMq.Delimiter);
				bool isTextSucceeded = data[1] == ConstantLibrary.RabbitMq.TextSuccessMarker.Status.True;
				if (!isTextSucceeded)
				{
					Console.WriteLine("Succeeded text count reset");
					_succeededTextCount = 0;
				}

				Console.WriteLine("----------");
			});

			Console.WriteLine("TextProcessingLimiter has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}
	}
}
