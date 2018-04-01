using System;
using RabbitMqLibrary;
using RedisLibrary;

namespace VowelConsonantRater
{
	class Program
	{
		private const string queueName = "vowel-cons-rater";
		private const string exchangeName = "vowel-cons-counter";

		static void Main()
		{
			var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare(queueName);
			rabbitMq.ExchangeDeclare(exchangeName, ExchangeType.Direct);
			rabbitMq.BindQueueToExchange(exchangeName);
			rabbitMq.ConsumeQueue(textId =>
			{
				Redis.Instance.SetDatabase(Redis.Instance.CalculateDatabase(textId));
				string countDataString = Redis.Instance.Database.StringGet($"{ConstantLibrary.Redis.Prefix.Count}{textId}");
				Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Count}{textId}: {countDataString}' from redis database({Redis.Instance.Database.Database})");
				string[] countData = countDataString.Split('|');
				int vowelCount;
				int consonantCount;
				if (Int32.TryParse(countData[0], out vowelCount) && Int32.TryParse(countData[1], out consonantCount))
				{
					string rank = CalculateRank(vowelCount, consonantCount);
					Redis.Instance.Database.StringSet($"{ConstantLibrary.Redis.Prefix.Rank}{textId}", rank);
					Console.WriteLine($"'{ConstantLibrary.Redis.Prefix.Rank}{textId}: {rank}' to redis database({Redis.Instance.Database.Database})");
				}
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
