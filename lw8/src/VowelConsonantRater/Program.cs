using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace VowelConsonantRater
{
	class Program
	{
		private const string _queueName = "vowel-cons-rater";
		private const string _listeningExchangeName = "vowel-cons-counter";
		private const string _publishExchangeName = "text-rank-calc";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare(_queueName);
			rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Direct);
			rabbitMq.BindQueueToExchange(_listeningExchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Console.WriteLine($"New message from {_listeningExchangeName}: \"{textId}\"");
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				string countKey = $"{ConstantLibrary.Redis.Prefix.Count}{textId}";
				string countDataString = Redis.Instance.Database.StringGet(countKey);
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Count}{textId}: {countDataString}' from redis database({Redis.Instance.Database.Database})");
				Redis.Instance.Database.KeyDelete(countKey);
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Count}{textId}: {countDataString}' deleted from redis database({Redis.Instance.Database.Database})");
				string[] countData = countDataString.Split('|');
				int vowelCount;
				int consonantCount;
				if (Int32.TryParse(countData[0], out vowelCount) && Int32.TryParse(countData[1], out consonantCount))
				{
					string rank = CalculateRank(vowelCount, consonantCount);

					Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{textId}: {rank}' to redis database({Redis.Instance.Database.Database})");
					Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Rank}{textId}", rank);

					Console.WriteLine($"{textId} to {_publishExchangeName} exchange");
					rabbitMq.PublishToExchange(_publishExchangeName, textId);
				}

				Console.WriteLine("----------");
			});

			Console.WriteLine("VowelConsonantRater has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();
		}

		private static string CalculateRank(int vowelCount, int consonantCount)
		{
			return consonantCount == 0 ? "Infinity" : ((double)vowelCount / consonantCount).ToString();
		}
	}
}
